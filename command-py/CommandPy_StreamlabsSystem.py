#XXX bug: [FIXED] reloading the script in the script manager freezes the interface, user is required to terminate the application
#XXX bug: init does not pay regard to the current enabledness of the script

#XXX bug: [FIXED] updated paths to reflect new streamlabs system layout
#XXX version 1.1.0.1: Fixed errors caught by github's code scanning.
#XXX version 1.0.1.0: changes to be compatible with version 1.0.2.29 of the chatbot
#XXX version 1.0.0.0: added a followage command (that uses the "new twitch api")


#from enum import Enum
#from enum import IntFlag
import datetime
import threading
import time
import urllib
import json
#import os.path
import dateutil.parser
import pytz
import re

#--------------------#
# Script Information #
#      &Globals      #
#--------------------#
ScriptName = "CommandPy"
Website = "https://www.emptysora.com/sorabot/"
Description = "Allows configuration of and scripting of custom commands in python."
Creator = "EmptySora_"
Version = "1.1.0.1"

CommandSet = []
MessageScannerQueue = []
MessageScannerQueueB = []
MessageScannerLock = threading.Lock()
MessageScannerLockB = threading.Lock()
MessageScannerTerminate = False
MessageScannerThread = None
ObjectSpeedrun = None
ExecState = True
LeaderBoardRegex = None
API_KEY = "YOUR TWITCH CLIENT API KEY HERE"
USER_NAME = "emptysora_"
SETTINGS_PATH = "PATH TO SETTINGS FILE HERE" #include terminal "\"
BURLS_PATH = "PATH TO BANNED URLS FILE HERE"

#--------------------#
#        Init        #
#--------------------#

# Called only when initializing the script
def Init():
    global ObjectSpeedrun
    ObjectSpeedrun = SpeedRun()
    Parent.Log(ScriptName,"Starting up "+ScriptName)
    Parent.Log(ScriptName,"Loading Commands")
    LoadCommands()
    LoadAppSettings()
    CreateSettingsJSON()
    Parent.Log(ScriptName,"Starting Scanner")
    MessageScanner()
    return

  
#--------------------#
#        Exec        #
#--------------------#



#  0 = Success
# -1 = Failure, Command Reported Failure
# -2 = Invalid Permissions
# -3 = Cooldown
# -4 = Usage Mask failure
# -5 = Not Enough Points
def Execute(data):
    #Parent.Log(ScriptName,"Received Message: " + data.Message)
    #Parent.Log(ScriptName,"Received Message (Raw): " + data.RawData)
    if not ExecState:
        return
    if data.IsFromTwitch() and data.IsChatMessage():
        AddMessageToScan(data)
    #regular message processing
    cmdname = data.GetParam(0).lower()
    for command in CommandSet:
        if command.Command.lower() == cmdname:
            result = command.Execute(data)
            break
    return

def ReloadSettings(jsonData):
    LoadAppSettings()
    #execute json reloading here
    #executes when the user clicks "Save Settings"
    return

def Unload():
    global MessageScannerTerminate
    MessageScannerTerminate = True
    #MessageScannerThread.join() #wait for child thread to quit
    #switched to daemon thread, daemon threads will be killed automatically upon exit
    #hence, no need to wait for join.
    return

def ScriptToggled(state):
    global ExecState
    ExecState = state
    return

def Tick():
    #pop off messages from the loopback queue, that way, we execute them on the proper thread
    #this avoids any thread race conditions
    global MessageScannerQueueB
    global MessageScannerLockB
    MessageScannerLockB.acquire(True)
    ln = len(MessageScannerQueueB)
    MessageScannerLockB.release()
    while ln > 0:
        MessageScannerLockB.acquire(True)
        message = MessageScannerQueueB.pop(0) #unshift first value
        MessageScannerLockB.release()
    
        Parent.SendStreamMessage(message)
    
        MessageScannerLockB.acquire(True)
        ln = len(MessageScannerQueueB)
        MessageScannerLockB.release()
    return





def FindCommandByComName(cname):
    global CommandSet
    for com in CommandSet:
        if com.CommandName == cname:
            return com
    return None

def LoadAppSettings():
    global CommandSet
    global SETTINGS_PATH
    Parent.Log(ScriptName,"Reloading Script Settings")
    if not os.path.isfile(SETTINGS_PATH + "settings.json"):
        return # No settings file, quit
    contents = ""
    with open(SETTINGS_PATH + "settings.json","r") as fh:
        contents = fh.read()
    if len(contents) > 3: # test for Byte order mark
        if [ord(contents[0]), ord(contents[1]), ord(contents[2])] == [0xEF, 0xBB, 0xBF]:
            contents = contents[3:] # skip BOM
    js = json.loads(contents)
    for k,v in js.iteritems():
        parts = k.split("_") # 0 = id "COMPY", 1 = type, 2 = CommandName value, 3 = PropName
        if not len(parts) == 4:
            continue
        if not parts[0] == "COMPY":
          continue
        FindCommandByComName(parts[2])[parts[3]] = v
    return

def CreateSettingsJSON():
    config = GenerateConfiguration()
    with open(SETTINGS_PATH + "Generated_UI_Config.json","w") as fh:
        fh.write(config)
    return

def MessageScanner():
    global MessageScannerThread
    def MessageScannerMain():
        global MessageScannerQueue
        global MessageScannerLock
        global MessageScannerQueueB
        global MessageScannerLockB
        global MessageScannerTerminate
        # messages in form [username, message]
        #pop(0)
        Parent.Log(ScriptName,"Started Message Scanner Thread")
        while True:
            if MessageScannerTerminate:
                break
            time.sleep(1)
            MessageScannerLock.acquire(True)
            ln = len(MessageScannerQueue)
            MessageScannerLock.release()
            while ln > 0:
                MessageScannerLock.acquire(True)
                messagez = MessageScannerQueue.pop(0) #unshift first value
                MessageScannerLock.release()

                message = messagez[1]
                user = messagez[0]
                if scan_message(messagez[2],message):
                    MessageScannerLockB.acquire(True)
                    MessageScannerQueueB.append("/timeout " + user + " 1") #purge the offender
                    MessageScannerQueueB.append("@" + user + " shortened URLs are not allowed in this channel!")
                    MessageScannerLockB.release()
                MessageScannerLock.acquire(True)
                ln = len(MessageScannerQueue)
                MessageScannerLock.release()
        return
    thread = threading.Thread(None, MessageScannerMain, "MessageScanner Thread")
    thread.daemon = True
    thread.start()
    MessageScannerThread = thread
    return

def scan_message(user,message):
    if Parent.HasPermission(user,"moderator",""): #Mod check hack
        return False
    Parent.Log(ScriptName,"Scanning Message: "+message)
    urls = GetBlockedURLS()
    words = message.split(" ")
    for word in words:
        for url in urls:
            if word.startswith(url) or word.startswith("https://" + url) or word.startswith("http://" + url):
                return True
    return False

def GetBlockedURLS():
    global BURLS_FILE
    path = BURLS_FILE
    with open(path) as fh:
        return [line.strip() for line in fh]

def BlockURL(url):
    path = "banned_urls.txt"
    with open(path,"a") as fh:
      fh.write(url+"\r\n")
    return
  
def AddMessageToScan(data):
    global MessageScannerQueue
    global MessageScannerLock
  
    MessageScannerLock.acquire(True)
    MessageScannerQueue.append([Parent.GetDisplayName(data.User),data.Message,data.User])
    MessageScannerLock.release()
    return

class SpeedRun:
  def __init__(self):
    return
  
  def GetCurrentRecord(self, categoryName):
    status = OldStreamStatus()
    if status is None:
      return "Unable to lookup World Record, currently offline!"
    return self.GetWorldRecord(status["game"],categoryName)
  
  def GetCurrentSpeedrun(self, categoryName,ranking):
    status = OldStreamStatus()
    if status is None:
      return "Unable to lookup World Record, currently offline!"
    gameName = status["game"]
    sptext = self.GetSpeedrun(gameName,categoryName,ranking)
    ordinal = self.OrdinalHelper(ranking)
    if sptext is None:
      return "Game \"" + gameName + "\" was not found!"
    elif sptext == False:
      return "Category \"" + categoryName + "\" was not found! Available Categories: " + self.__getCategories(self.__getGameID(gameName)[0])
    elif sptext == -1:
      return "No runs found!"
    else:
      return str(ranking) + ordinal + " Place Record for " + gameName + " " + categoryName + " is " + sptext
  
  def GetCurrentGameSpeedrun(self,gameName,categoryName,ranking):
    sptext = self.GetSpeedrun(gameName,categoryName,ranking)
    ordinal = self.OrdinalHelper(ranking)
    if sptext is None:
      return "Game \"" + gameName + "\" was not found!"
    elif sptext == False:
      return "Category \"" + categoryName + "\" was not found! Available Categories: " + self.__getCategories(self.__getGameID(gameName)[0])
    elif sptext == -1:
      return "No runs found!"
    else:
      return str(ranking) + ordinal + " Place Record for " + gameName + " " + categoryName + " is " + sptext
  
  def OrdinalHelper(self,number):
    if number < 0:
      number = number * -1
    if number == 0:
      return "th"
    else:
      cases = {
        1: "st",
        2: "nd",
        3: "rd"
      }
      number = number % 100
      if number >= 10 and number <= 19:
        return "th" #I hate this...
      else:
        number = number % 10
        return cases.get(number,"th")
  
  def GetUptime(self):
    status = OldStreamStatus()
    if status is None:
      return "Stream is not currently live!"
    if "created_at" in status:
      status = status["created_at"]
    elif "started_at" in status:
      status = status["started_at"]
    else:
      return "Stream is not currently live!"
    #um, it seems that twitch changed "created_at" to "started_at"
    dt = datetime.datetime.strptime(status,"%Y-%m-%dT%H:%M:%SZ")
    ep = int(dt.strftime("%s"))
    now = int(datetime.datetime.now().strftime("%s"))
    diff = now - ep
    return "Current uptime is: " + self.__formatTime(diff)
  
  def GetWorldRecord(self,gameName,categoryName):
    sptext = self.GetSpeedrun(gameName,categoryName,1)
    if sptext is None:
      return "Game \"" + gameName + "\" was not found!"
    elif sptext == False:
      return "Category \"" + categoryName + "\" was not found! Available Categories: " + self.__getCategories(self.__getGameID(gameName)[0])
    elif sptext == -1:
      return "No runs found!"
    else:
      return "World Record for " + gameName + " " + categoryName + " is " + sptext
  
  def GetSpeedrun(self, gameName,catName,ranking):
    gid = self.__getGameID(gameName)
    if gid is None:
      return None
    gname = gid[1]
    gid = gid[0]
    cid = self.__getCategoryID(gid,catName)
    if cid is None:
      return False
    lb = self.__internalGetLeaderboard(gid,cid)
    
    for runx in lb:
      run = runx
      if run["place"] == ranking:
        run = run["run"]
        player = self.__getUserName(run["players"][0]["uri"])
        date = run["date"]
        time = self.__formatTime(run["times"]["primary_t"])
        status = run["status"]["status"]
        verifier = self.__getUserName("http://www.speedrun.com/api/v1/users/" + run["status"]["examiner"])
        link = ""
        if len(run["videos"]["links"]) > 0:
          link = run["videos"]["links"][0]["uri"]
        result = time + " by " + player + " on " + date
        if status == "verified":
          result += " (verified by "+ verifier + ")"
        if not link == "":
          result += " [" + link + "]"
        return result
    return -1
  
  def __formatTime(self, fTime):
    dec = fTime % 1
    whole = fTime - dec
    hours = 0
    minutes = 0
    seconds = whole
    milliseconds = dec
    if seconds > 60:
      temp = seconds % 60
      minutes = (seconds - temp) / 60
      seconds = temp
    if minutes > 60:
      temp = minutes % 60
      hours = (minutes - temp) / 60
      minutes = temp
    hours = "" + str(hours)
    minutes = "" + str(minutes)
    seconds = "" + str(seconds)
    milliseconds = "" + str(milliseconds)
    while len(milliseconds) < 3:
      milliseconds = "0" + milliseconds
    while len(seconds) < 2:
      seconds = "0" + seconds
    while len(minutes) < 2:
      minutes = "0" + minutes
    while len(hours) < 3:
      hours = "0" + hours
    return hours + ":" + minutes + ":" + seconds + "." + milliseconds
  
  def __getUserName(self, apiURL):
    return jcurl("GET",apiURL,None,None,None)["data"]["names"]["international"]

  def __getGameID(self, gameName):
    #Parent.Log(ScriptName,"Getting Game ID: "+ gameName)
    data = jcurl("GET","http://www.speedrun.com/api/v1/games",{"name": gameName},None,None)["data"]
    if len(data) == 0:
      return None
    #Parent.Log(ScriptName,"Getting Game ID: "+ str(data[0]))
    name = None
    tname = data[0]["names"]["twitch"]
    iname = data[0]["names"]["international"]
    if not iname is None:
      name = iname
    elif not tname is None:
      name = tname
    else:
      name = gameName
    return [data[0]["id"], name]
  
  def __getCategoryID(self, gameID, catName):
    data = jcurl("GET","http://www.speedrun.com/api/v1/games/" + gameID + "/categories",None,None,None)["data"]
    if len(data) == 0:
      return None
    Parent.Log(ScriptName,"Getting CatID: "+ str(data[0]))
    for dat in data:
      if dat["name"] == catName:
        return dat["id"]
    return None
  
  def __getCategories(self, gameID):
    data = jcurl("GET","http://www.speedrun.com/api/v1/games/" + gameID + "/categories",None,None,None)["data"]
    if len(data) == 0:
      return None
    Parent.Log(ScriptName,"Getting CatIDs: "+ str(data[0]))
    ret = []
    for dat in data:
      ret.append(dat["name"])
    return ", ".join(ret)
  
  def __internalGetLeaderboard(self, gameID, catID):
    return jcurl("GET","http://www.speedrun.com/api/v1/leaderboards/" + gameID + "/category/" + catID,None,None,None)["data"]["runs"]


class PythonCommand:
    Execution = None # a lambda or function expression that takes the following arguments (PythonCommand, Data)
    Command = "!demo"
    Cooldown = 0 #seconds
    UserCooldown = 0 #seconds
    Permission = "everyone"
    Info = ""
    Cost = 0
    Enabled = True
    UsageMask = 0
    CommandName = ""
    #See Below
    def __setitem__(self,key,value):
      mux = {"Twitch Chat":5,"Twitch Whisper":9,"Discord Chat":6,"Discord Whisper":10,"Both Chat":7,"Both Whisper":11,"Twitch Both":13,"Discord Both":14,"All":15}
      if key=="Execution":
        self.Execution = value
      elif key=="Command":
        self.Command = value
      elif key=="Cooldown":
        self.Cooldown = value
      elif key=="UserCooldown":
        self.UserCooldown = value
      elif key=="Permission":
        self.Permission = value
      elif key=="Info":
        self.Info = value
      elif key=="Cost":
        self.Cost = value
      elif key=="Enabled":
        self.Enabled = value
      elif key=="UsageMask":
        self.UsageMask = mux[value]
      elif key=="CommandName":
        self.CommandName = value
      return
    def __init__(self,cb,cname,com,cool,ucool,perm,inf,cost,enabled,mask):
      self.Execution = cb
      self.Command = com
      self.Cooldown = cool
      self.UserCooldown = ucool
      self.Permission = perm
      self.Info = inf
      self.Cost = cost
      self.Enabled = enabled
      self.UsageMask = mask
      self.CommandName = cname
      return
    def SetCooldown(self,data):
      if self.Cooldown > 0:
        Parent.AddCooldown(ScriptName,self.Command,self.Cooldown)
      if self.UserCooldown > 0:
        Parent.AddUserCooldown(ScriptName,self.Command,data.User,self.UserCooldown)
      return
    #Command Matching must be done outside of execute
    #the return value of execute merely indicates whether or not the command executed
    #  0 = Success
    # -1 = Failure, Command Reported Failure
    # -2 = Invalid Permissions
    # -3 = Cooldown
    # -4 = Usage Mask failure
    # -5 = Not Enough Points
    def Execute(self,data):
      if not self.CheckUsageMask(data):
        Parent.Log(ScriptName,"Command failed: usage mask conflict")
        return -4
      elif not self.CheckCooldowns(data):
        Parent.Log(ScriptName,"Command failed: on cooldown")
        return -3
      elif not self.CheckPermission(data):
        Parent.Log(ScriptName,"Command failed: permission conflict")
        return -2
      elif self.Cost > 0:
        # I'm not sure if this will work, the update specifies a username parameter, but I'm not sure what it is for.
        if not Parent.RemovePoints(data.User,data.UserName,self.Cost):
          Parent.Log(ScriptName,"Command failed: not enough currency")
          return -5
      retval = self.Execution(data)
      self.SetCooldown(data)
      return retval
    
    def CheckPermission(self,data):
      if not Parent.HasPermission(data.User,self.Permission,self.Info):
        return False
      return True
    
    def CheckCooldowns(self,data):
      if data.User == USER_NAME:
        return True
      if Parent.IsOnCooldown(ScriptName,self.Command):
        return False
      elif Parent.IsOnUserCooldown(ScriptName,self.Command,data.User):
        return False
      return True
    
    def CheckUsageMask(self,data):
      #returns a boolean value indicating whether the usage mask matches the command
      if data.IsFromTwitch():
        if not UnMask(UsageMasks.Twitch,self.UsageMask):
          return False
      elif data.IsFromDiscord():
        if not UnMask(UsageMasks.Discord,self.UsageMask):
          return False
      if data.IsChatMessage():
        if not UnMask(UsageMasks.Chat,self.UsageMask):
          return False
      elif data.IsWhisper():
        if not UnMask(UsageMasks.Whisper,self.UsageMask):
          return False
      return True

def UnMask(a,b):
  return (a & b) == a

class UsageMasks:
  Twitch = 1
  Discord = 2
  Chat = 4
  Whisper = 8
  #pre defined
  TwitchChat = 5
  TwitchWhisper = 9
  DiscordChat = 6
  DiscordWhisper = 10
  BothChat = 7
  BothWhisper = 11
  TwitchBoth = 13
  DiscordBoth = 14
  All = 15

def SendMessage(data,msg):
  #provides a shorthand for sending responses from the bot, so that this block of code
  #doesn't have to be in every function that uses it
  #also enables sending more than one message more easily
  if data.IsFromTwitch():
    if data.IsWhisper():
      Parent.SendStreamWhisper(data.User,msg)
    elif data.IsChatMessage():
      Parent.SendStreamMessage(msg)
  elif data.IsFromDiscord():
    if data.IsWhisper():
      Parent.SendDiscordDM(data.User,msg)
    elif data.IsChatMessage():
      Parent.SendDiscordMessage(msg)
  return

def OldStreamStatus():
  if Parent.IsLive():
    try:
      stream = HelixAPI("GET","streams",{"user_id":157578041},None)
      stream = stream["data"][0]
      game = HelixAPI("GET","games",{"id":stream["game_id"]},None)
      game = game["data"][0]
      stream["game"] = game["name"]
      return stream
    except:
      return None
  else:
    return None

def curl(method,urlstr,query,post_data,headers):
  u = urlstr
  h = headers
  p = post_data
  if not query is None:
    u = urlstr.split("?")[0] + "?" + urllib.urlencode(query)
  #m,u,p,h
  if h is None: #headers
    h = {}
  if p is None: #post_data
    p = {}
  response = ""
  if method == "GET":
    response = Parent.GetRequest(u,h)
  elif method == "POST":
    response = Parent.PostRequest(u,h,p)
  elif method == "DELETE":
    response = Parent.DeleteRequest(u,h)
  elif method == "PUT":
    response = Parent.PutRequest(u,h,p)
  else:
    return None
  return response

def jcurl(m,u,q,p,h):
  string = curl(m,u,q,p,h)
  if string is None:
    return None
  return json.loads(json.loads(string)["response"])

def JoinParams(data):
  lst = []
  for i in range(1,data.GetParamCount()):
    lst.append(data.GetParam(i))
  return " ".join(lst)

def HelixAPI(method,endpoint,query_params,post_data):
  api = "https://api.twitch.tv/helix/" + endpoint
  headers = {"Client-ID":API_KEY}
  return jcurl(method,api,query_params,post_data,headers)

#a follows b
def Followage(requestingUser,targetUser,rUserID = False, tUserID = False, requestingUserName = None, targetUserName = None):
  orUser = requestingUserName or requestingUser
  otUser = targetUserName or targetUser
  if not rUserID:
    #get user id
    ru_response = json.loads((HelixAPI("GET","users",{"login":str(requestingUser)},None))["response"])["data"]
    found_user = False
    for i in range(0,len(ru_response)):
      xuser = ru_response[i]
      if xuser["login"] == str(requestingUser):
        found_user = True
        requestingUser = xuser["id"]
        break
    if not found_user:
      return {"error":True,"message":"User \"" + str(requestingUser) + "\" does not exist!"}
  if not tUserID:
    #get user id
    ru_response = json.loads((HelixAPI("GET","users",{"login":str(targetUser)},None))["response"])["data"]
    found_user = False
    for i in range(0,len(ru_response)):
      xuser = ru_response[i]
      if xuser["login"] == str(targetUser):
        found_user = True
        targetUser = xuser["id"]
        break
    if not found_user:
      return {"error":True,"message":"User \"" + str(targetUser) + "\" does not exist!"}
  response = json.loads((HelixAPI("GET","users/follows",{"to_id":targetUser,"from_id":requestingUser},None))["response"])
  Parent.Log(ScriptName,str(response))
  if response["total"] == 0:
    return {"error":True,"message":"User \"" + str(orUser) + "\" does not folow user \"" + str(otUser) + "\"!"}
  followed_at = dateutil.parser.parse(response["data"][0]["followed_at"]).astimezone(pytz.utc)
  now_time = datetime.datetime.now(pytz.utc)
  return {"error":False,"message":FormatTimeDifference(followed_at,now_time)}
  
  #response["data"][0]["followed_at"]
  #check if response["total"] > 0 however
def IsLeapYear(year):
  if not year % 4:
    return False
  elif not year % 100:
    return True
  elif not year % 400:
    return False
  else:
    return True

def DaysInMonth(month,year):
  feb = 28
  if IsLeapYear(year):
    feb = 29
  return ([31,feb,31,30,31,30,31,31,30,31,30,31])[month]
def PluralityChecker(n,qt):
  pl = ""
  if n > 1 or n == 0:
    pl = "s"
  return str(n) + " " + qt + pl

def FormatTimeDifference(time1,time2):
  b = time1
  a = time2
  if time1 > time2:
    b = time2
    a = time1
  years = a.year - b.year
  months = a.month - b.month #month between 1 and 12, this shouldn't be negative
  weeks = 0 #dynamic calc
  days = a.day - b.day #day between 1 and days in month
  hours = a.hour - b.hour  #between 0 and 23
  minutes = a.minute - b.minute #between 0 and 59
  seconds = a.second - b.second #between 0 and 59
  #balance
  if seconds < 0:
    seconds += 60
    minutes -= 1
  if minutes < 0:
    minutes += 60
    hours -= 1
  if hours < 0:
    hours += 24
    days -= 1
  if days < 0:
    months -= 1
    days += DaysInMonth(a.month - 1, a.year)
    # March 11, April 10 (10-11 = -1; dim(march) = 31; 31 + -1 = 30)
  if months < 0:
    months += 12
    years -= 1
  #while days >= 7:
  #  days -= 7
  #  weeks +=1
  #we have to do this backwards
  ret = []
  if years > 0:
    ret.append(PluralityChecker(years,"year"))
  if months > 0:
    ret.append(PluralityChecker(months,"month"))
  #if weeks > 0:
  #  ret.append(PluralityChecker(weeks,"week"))
  if days > 0:
    ret.append(PluralityChecker(days,"day"))
  if hours > 0:
    ret.append(PluralityChecker(hours,"hour"))
  if minutes > 0:
    ret.append(PluralityChecker(minutes,"minute"))
  if seconds > 0:
    ret.append(PluralityChecker(seconds,"second"))

  return ", ".join(ret) + " (followed on " + b.strftime("%m/%d/%Y at %H:%M:%S") + " UTC)"




def Command_Followage(data):
  x = None
  if data.User.lower() == USER_NAME:
    #x = Followage(requestingUser = data.User,targetUser = "geoff",requestingUserName = data.UserName, targetUserName = "Geoff")
    return 0
  else:
    x = Followage(requestingUser = data.User,targetUser = USER_NAME, requestingUserName = data.UserName, targetUserName = USER_NAME)
  if x["error"]:
    SendMessage(data,x["message"])
  else:
    SendMessage(data,data.UserName + ": You have been following " + USER_NAME + " for " + x["message"])
  return 0

def Command_Day(data):
  msg = ""
  day = datetime.datetime.today().weekday() #0 - 6 Mon - Sun
  if day == 0:
    msg="Today is Monday! WutFace"
  elif day == 1:
    msg="Today is Tuesday! 4Head"
  elif day == 2:
    msg="Today is Wednesday! twitchRaid"
  elif day == 3:
    msg="Today is Friday-Eve! Kappa (Thursday)"
  elif day == 4:
    msg="Today is Friday! PogChamp"
  elif day == 5:
    msg="Today is Saturday! Kreygasm"
  else:
    msg="Today is Sunday! ResidentSleeper"
  
  SendMessage(data,msg)
  return 0

def Command_StreamStatus(data):
  msg = ""
  if Parent.IsLive():
    msg = "Stream is not currently live!"
  else:
    msg = "Stream is currently live!"
  SendMessage(data,msg)
  return 0

def Command_BlockURL(data):
  if data.GetParamCount() < 2:
    return -1 #not enough parameters
  BlockURL(data.GetParam(1).lower())
  return 0

def Command_WorldRecord(data):
  if data.GetParamCount() < 2:
    SendMessage(data,Parent.GetDisplayName(data.User) + ": you must specify a category!")
  else:
    SendMessage(data,Parent.GetDisplayName(data.User) + ": " + ObjectSpeedrun.GetCurrentRecord(JoinParams(data)))
  return 0

def Command_LeaderBoard(data):
  #Arguments: category, [ranking = 1]
  #Usage: !leaderboard "category" [ranking = 1]
  #       !leaderboard "game" "category" [ranking = 1]
  if data.GetParamCount() < 2:
    Parent.Log(ScriptName,"ERROR0: "+ data.Message)
    SendMessage(data,Parent.GetDisplayName(data.User) + ": you must specify a category!")    
  else:
    result = LeaderBoardRegex.search(FixQuotes(data.Message))
    if result:
      full = result.group("full")
      if not full == data.Message:
        Parent.Log(ScriptName,"ERROR1: "+ data.Message + "; " + full)
        SendMessage(data,Parent.GetDisplayName(data.User) + ": The syntax of this command is \"!leaderboard [\"game\"] \"category\" [ranking]\"")
      else:
        groups = result.groupdict()
        arg_one = groups["argone"]
        arg_two = groups["argtwo"]
        arg_three = groups["argthree"]
        #Values will be "None" if they aren't matched
        game = None
        category = None
        place = 1
        response = ""
        if not arg_two is None:
          game = arg_one
          category = arg_two
        else:
          category = arg_one
        if not arg_three is None:
          try:
            place = int(arg_three)
          except:
            place = 1
        if game is None:
          response = ObjectSpeedrun.GetCurrentSpeedrun(category,place)
        else:
          response = ObjectSpeedrun.GetCurrentGameSpeedrun(game,category,place)
        SendMessage(data,Parent.GetDisplayName(data.User) + ": " + response)
    else:
      Parent.Log(ScriptName,"ERROR2: "+ data.Message)
      SendMessage(data,Parent.GetDisplayName(data.User) + ": The syntax of this command is \"!leaderboard [\"game\"] \"category\" [ranking]\"")
      #(?P<full>(\![^\s]+[\s]+)([\"\'](?P<argone>[^\"\']+)[\"\'])([\s]+[\"\'](?P<argtwo>[^\"\']+)[\"\'])?([\s]+(?P<argthree>[0123456789]+))?[\s]*)
  
  # string SpeedRun.GetCurrentRecord(string category_name)
  # string SpeedRun.GetWorldRecord(string game_name, string category_name)
  # string SpeedRun.GetSpeedrun(string game_name, string category_name, int ranking)
  # string SpeedRun.GetCurrentSpeedrun(string categoryName, int ranking)
  # string SpeedRun.GetCurrentGameSpeedrun(string gameName, string categoryName, int ranking)
  
  return 0


def FixQuotes(quoted):
    return quoted.replace("\u201C","\"") \
        .replace("\u201D","\"") \
        .replace("\u2018","'") \
        .replace("\u2019","'")

def LoadCommands():
  global CommandSet
  global LeaderBoardRegex
  global ObjectSpeedrun

  if ObjectSpeedrun is None:
    ObjectSpeedrun = SpeedRun()
  
  LeaderBoardRegex = re.compile(r"(?P<full>(\![^\s]+[\s]+)((?P<quotea>[\"\'])(?P<argone>(.(?!(?P=quotea)))*.(?=(?P=quotea)))(?P=quotea))([\s]+(?P<quoteb>[\"\'])(?P<argtwo>(.(?!(?P=quoteb)))*.(?=(?P=quoteb)))(?P=quoteb))?([\s]+(?P<argthree>[0123456789]+))?[\s]*)", re.U)
  #here is where you would register commands
  #CommandSet.append(PythonCommand(cb,cname,com,cool,ucool,perm,inf,cost,enabled,mask))
  #also note that these settings are XXX DEFAULT XXX
  CommandSet.append(PythonCommand(Command_Day,"Day","!day",15,15,"everyone","",0,True,UsageMasks.BothChat))
  CommandSet.append(PythonCommand(Command_StreamStatus,"Status","!stream_status",10,10,"everyone","",0,True,UsageMasks.BothChat))
  CommandSet.append(PythonCommand(Command_BlockURL,"BlockURL","!block_url",0,0,"moderator","",0,True,UsageMasks.BothChat))
  CommandSet.append(PythonCommand(Command_WorldRecord,"WR","!wr",15,15,"everyone","",0,True,UsageMasks.TwitchChat))
  CommandSet.append(PythonCommand(Command_Followage,"Followage","!followage",60,600,"everyone","",0,True,UsageMasks.TwitchChat))
  CommandSet.append(PythonCommand(Command_LeaderBoard,"Leaderboard","!leaderboard",1,1,"everyone","",0,True,UsageMasks.BothChat))
  #Command_LeaderBoard
  
  # string SpeedRun.GetCurrentRecord(string category_name)
  # string SpeedRun.GetWorldRecord(string game_name, string category_name)
  # string SpeedRun.GetSpeedrun(string game_name, string category_name, int ranking)
  # string SpeedRun.GetCurrentSpeedrun(string categoryName, int ranking)
  # string SpeedRun.GetCurrentGameSpeedrun(string gameName, string categoryName, int ranking)
  #doing this because we can actually run these four commands (and probably even more) using pure command syntax in SLCB
  return


def listdict(lst):
  ret = {}
  for item in lst:
    ret[item]=item
  return ret
#TODO
#list = [val,val]
#length = len(list)
#list.append
