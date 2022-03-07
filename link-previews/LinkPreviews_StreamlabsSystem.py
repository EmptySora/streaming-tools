#XXX version 1.0.0.0: Initial version of the script


#from enum import Enum
#from enum import IntFlag
#import datetime
import threading
import time
#import urllib
import json
#import os.path
import re
import http.client
#from HTMLParser import HTMLParser
import urlparse
import dateutil.parser

#--------------------#
# Script Information #
#      &Globals      #
#--------------------#
ScriptName = "LinkPreviews"
Website = "https://www.emptysora.com/sorabot/"
Description = "Displays previews for certain links in twitch chat."
Creator = "EmptySora_"
Version = "1.0.0.0"

CommandSet = []
MessageScannerQueue = []
MessageScannerQueueB = []
MessageScannerLock = threading.Lock()
MessageScannerLockB = threading.Lock()
MessageScannerTerminate = False
MessageScannerThread = None
ExecState = True
URLs = []

API_KEY = "YOUR YOUTUBE API KEY"
API_SERVICE_NAME = "youtube"
API_VERSION = "v3"

#--------------------#
#        Init        #
#--------------------#

# Called only when initializing the script
def Init():
	Parent.Log(ScriptName,"Starting up "+ScriptName)
	LoadURLs()
	Parent.Log(ScriptName,"Starting Scanner")
	MessageScanner()
	return

	
#--------------------#
#        Exec        #
#--------------------#

def Execute(data):
	if not ExecState:
		return
	if data.IsFromTwitch() and data.IsChatMessage():
		AddMessageToScan(data)
	return

def Unload():
	MessageScannerTerminate = True
	#MessageScannerThread.join() #wait for child thread to quit
	#switched to daemon thread, daemon threads will be killed automatically upon exit
	#hence, no need to wait for join.
	return

def ScriptToggled(state):
	global ExecState
	ExecState = state
	return

#this function appears to send messages back to the chat.
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


def MessageScanner():
	global MessageScannerThread
	def MessageScannerMain():
		global MessageScannerQueue
		global MessageScannerLock
		global MessageScannerQueueB
		global MessageScannerLockB
		global MessageScannerTerminate
		# messages in form [username, message]
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
				message = MessageScannerQueue.pop(0) #unshift first value
				MessageScannerLock.release()
				result = scan_message(message)
				Parent.Log(ScriptName,"Message Scan: "+result.__str__())
				if not (result == False):
					# YoutubeAPIRequest(GetYoutubeVideoID(url))
					if result.lower().startswith("youtu.be") or result.lower().startswith("youtube.com"):
					  yt_id = GetYoutubeVideoID(result)
					  Parent.Log(ScriptName,"Video ID: "+yt_id)
					  api_result = YoutubeAPIRequest(yt_id)["items"][0]
					  
					  x_duration = ParseDuration(api_result["contentDetails"]["duration"])
					  x_title = api_result["snippet"]["title"]
					  x_channel = api_result["snippet"]["channelTitle"]
					  x_upload = dateutil.parser.parse(api_result["snippet"]["publishedAt"]).strftime("%m/%d/%Y at %H:%M:%S")
					  x_views = api_result["statistics"]["viewCount"]
					  x_likes = api_result["statistics"]["likeCount"]
					  x_dislikes = api_result["statistics"]["dislikeCount"]
					  x_faves = api_result["statistics"]["favoriteCount"]
					  x_comments = api_result["statistics"]["commentCount"]
					  
					  MessageScannerLockB.acquire(True)
					  MessageScannerQueueB.append("YouTube Video (" + yt_id + "): \"" + x_title  + "\" uploaded by \"" + x_channel + "\" (" + x_upload + ") [duration: " + x_duration + "; " + x_views + " views; " + x_likes + "/" + x_dislikes + " likes/dislikes]")
					  #MessageScannerQueueB.append("YouTube Video (" + yt_id + "): ")
					  MessageScannerLockB.release()
					#elif result[0] == "twitter.com":
					#  
				MessageScannerLock.acquire(True)
				ln = len(MessageScannerQueue)
				MessageScannerLock.release()
		return
	thread = threading.Thread(None, MessageScannerMain, "MessageScanner Thread")
	thread.daemon = True
	thread.start()
	MessageScannerThread = thread
	return

def ParseDuration(duration_string):
  Parent.Log(ScriptName,"Parsing Duration: "+duration_string)
  match = re.match(r'P((?P<years>\d+)Y)?((?P<months>\d+)M)?((?P<weeks>\d+)W)?((?P<days>\d+)D)?(T((?P<hours>\d+)H)?((?P<minutes>\d+)M)?((?P<seconds>\d+)S)?)?',duration_string).groupdict()
  years = int(match['years'] or 0)
  months = int(match['months'] or 0)
  weeks = int(match['weeks'] or 0)
  days = int(match['days'] or 0)
  hours = int(match['hours'] or 0)
  minutes = int(match['minutes'] or 0)
  seconds = int(match['seconds'] or 0)
  while seconds > 60:
    seconds = seconds - 60
    minutes = minutes + 1
  while minutes > 60:
    minutes = minutes - 60
    hours = hours + 1
  days = days + (weeks * 7) + (months * 30) + (years * 365)
  hours = hours + (days * 24)
  response = "{h:{fill}{align}{width}}:{m:{fill}{align}{width}}:{s:{fill}{align}{width}}".format(d = days,h = hours,m = minutes,s = seconds, fill = '0', width = 2, align = '>')
  Parent.Log(ScriptName,"Formatting Results: "+response)
  return response
  

#returns false if the message does not contain the target urls
#returns the full url if it does.
#index 0: contains host
#index 1: contains header list
#index 2: contains response body
def scan_message(message):
  Parent.Log(ScriptName,"Scanning Message: "+message)
  words = message.split(" ")
  for url in URLs:
    #reg = re.compile(re.escape(url),re.IGNORECASE)
    for word in words:#flipping them results in less compiles
      if word.lower().startswith(url): #HeadRequest(secure,host,requst)
        return word#HeadRequest(False,word,reg.sub('',word,1))
      elif word.lower().startswith("http://" + url):
        return word[7:]#HeadRequest(False,word[7:],reg.sub('',word[7:],1))
      elif word.lower().startswith("https://" + url):
        return word[8:]
      elif word.lower().startswith("http://www." + url):
        return word[11:]#HeadRequest(False,word[7:],reg.sub('',word[7:],1))
      elif word.lower().startswith("https://www." + url):
        return word[12:]
      elif word.lower().startswith("www." + url):
        return word[4:]#HeadRequest(True,word[8:],reg.sub('',word[8:],1))
  return False

def AddMessageToScan(data):
	global MessageScannerQueue
	global MessageScannerLock
	
	MessageScannerLock.acquire(True)
	MessageScannerQueue.append(data.Message)
	MessageScannerLock.release()
	return



def YoutubeAPIRequest(ID):
  url = "/youtube/v3/videos?key=" + API_KEY + "=snippet,contentDetails,statistics&id=" + ID + "&maxResults=1"
  conn = http.client.HTTPSConnection("www.googleapis.com",443) #open a HTTPS connection via port 443
  conn.putrequest('GET',url)
  conn.endheaders()
  return json.loads(conn.getresponse().read())


def GetYoutubeVideoID(url):
  Parent.Log(ScriptName,"Getting ID: "+url)
  if url.lower().startswith("youtube.com"):
    return urlparse.parse_qs(urlparse.urlparse(url).query)['v'][0]
  elif url.lower().startswith("youtu.be"):
    return url.split("/")[-1].split("?")[0] #take time off of end
  #THIS IS AN ERROR, NOTHING IS BEING RETURNED HERE...




def LoadURLs():
  #URLs.append("twitter.com")
  URLs.append("youtube.com")
  URLs.append("youtu.be")

# Twitter: 
#   Meta(property,content)
#     og:type == "video"
#     og:title == "[user name] on Twitter" (split by " " and take [0] for name)
#     og:description == "[post text description]"
#     og:video:width == "[video width]"
#     og:video:height == "[video height]"
