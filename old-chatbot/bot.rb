#require 'em/pure_ruby'
require 'twitch'
require 'inifile'
require 'pp'
require 'twitch/chat'
require 'socket'
require 'date'
require 'net/http'
require 'json'
require 'thread'
require 'date'
require 'active_support/all'
require 'rbconfig'
require 'win32/pipe'
include Win32
OpenSSL::SSL::VERIFY_PEER = OpenSSL::SSL::VERIFY_NONE

$path=File.expand_path(File.dirname(__FILE__))

$file = IniFile.load($path+"/config.ini")

$config = {
  client_id: $file["Client"]["ID"],
  secret_key: $file["Client"]["Secret"],
  redirect_uri:"http://localhost:50682",
  scope: ["channel_check_subscription","channel_commercial","channel_editor","channel_feed_edit","channel_feed_read","channel_read","channel_stream","channel_subscriptions","chat_login","collections_edit","communities_edit","communities_moderate","user_blocks_edit","user_blocks_read","user_follows_edit","user_read","user_subscriptions","viewing_activity_read"]
}
token=""

@twitch=Twitch.new($config)

@twitch.link
@data=@twitch.auth(ARGV[0])
token=@data[:body]["access_token"]


Thread.new { #SoraBlanks currency gen; 1 SoraBlank every minute of viewership
	puts "In thread"
	while true do
		begin
			while streamStatus()==nil
				puts "Sleeping for 5 minutes, stream is offline"
				sleep(5*60)
			end
			viewers=get_viewers
			users=viewers.count
			vwrs={}
			viewers.each { |user| vwrs[user]=1; }
			curlJSON("PUT","http://localhost:40000/Blanks",nil,vwrs.to_json,nil)
			puts "Gave #{users} Viewers each 1 SoraBlank"
			sleep(60) #Sleep for designated interval (60 seconds)
		rescue
			sleep(60) #Sleep for designated interval (60 seconds)
		end
	end
}
def getBlanks(user)
	return curlJSON("GET","http://localhost:40000/Blanks/#{user}",nil,nil,nil)["data"]
end
def setBlanks(user,xblanks)
	qp={}
	qp["blanks"]=xblanks;
	curlJSON("PUT","http://localhost:40000/Blanks/#{user}",qp,nil,nil)
end
def redeem(user,client,item)
	success=false
	cost=0
	description=""
	begin
		case item
			when "invite" #cost = 180 ; 3 Hours
				cost=180
				description="discord invite"
			else
				client.send_message "#{user}: \"#{item}\" is not a valid redeemable!"
				return nil
		end
	rescue
	end
	qp={}
	qp["cost"]=cost
	tmp=curlJSON("GET","http://localhost:40000/Purchase/#{user}",qp,nil,nil)
	success=tmp["Status"]=="Success"
	userblanks=tmp["Data"]+cost
	
	begin
		case item
			when "invite" #cost = 180 ; 3 Hours
				if success
					invite=get_discord_invite()
					client.send_whisper(user,"Hey #{user}! Thanks for redeeming a discord invite! Open the link that follows to join! "+invite)
				end
		end
	rescue
	end
	
	if not success
		client.send_message("#{user}: You do not have enough Blanks to redeem a #{description}! Required: #{cost} (Current: #{userblanks})")
	else
		userblanks=userblanks-cost
		client.send_message("#{user}: Redeemed a #{description} for #{cost} blanks! You now have #{userblanks} Blanks!")
	end
end
def command_handle(user,message,command,arguments,client)
	#END REDEMABLES SECTION
  client.Log("INFO","BOT_COMMAND","Parsing command from #{user}: #{message}")
	cooloff=true
	cooldown=60
	get_users
	modded = user_is_mod(user)
	if not modded
		if is_command_on_cooldown(command)
      client.Log("INFO","BOT_COMMAND","#{user} failed to execute command because cooldown.")
			return true
		end
	end
		
	case command
		when "!add_command"
			if not modded
       	client.Log("INFO","BOT_COMMAND","#{user} attempted to run !add_command without authorization.")
				return true
			end
			#arguments
			comname=arguments[0]
			comtext=arguments
			comtext.shift
			comtext=comtext.join(" ")
			if not comname.start_with?("!")
				comname="!"+comname
			end
			isnew=AddCustomCommand(comname,comtext)
			if isnew
				client.send_message(user+", command \""+comname+"\" has been successfully added!")
			else
				client.send_message(user+", command \""+comname+"\" has been successfully modified!")
			end
		when "!delete_command"
			if not modded
       	client.Log("INFO","BOT_COMMAND","#{user} attempted to run !delete_command without authorization.")
				return true
			end
			comname=arguments[0]
			if not comname.start_with?("!")
				comname="!"+comname
			end
			if DeleteCustomCommand(comname)
				client.send_message(user+", command \""+comname+"\" has been successfully deleted!")
			else
				client.send_message(user+", cannot delete command \""+comname+"\" because it doesn't exist!")
			end
		when "!addquote"
			if not modded
				qnum=curlJSON("PUT","http://localhost:40000/Quote/Recommend",nil,arguments.join(" ")+" (Submitted by "+user+")",nil)["Data"]["number"]
				client.send_message(user+", added quote (#{qnum}) for recommendation!")
				return true
			end
			qnum=curlJSON("PUT","http://localhost:40000/Quote",nil,arguments.join(" "),nil)["Data"]["number"]
			client.send_message(user+", added quote (#{qnum})!")
		when "!block_url"
			if not modded
       	client.Log("INFO","BOT_COMMAND","#{user} attempted to run !block_url without authorization.")
				return true
			end
			block_url(arguments[1])
		when "!delquote"
			if not modded
       	client.Log("INFO","BOT_COMMAND","#{user} attempted to run !delquote without authorization.")
				return true
			end
			qnum=Integer(arguments[0])
			if curlJSON("DELETE","http://localhost:40000/Quote?id=#{qnum}",nil,nil,nil)["Status"]=="Success"
				client.send_message(user+", Quote #{qnum} has been removed.")
			else
				client.send_message(user+", The specified quote does not exist")
			end
		when "!pun"
			puns=get_puns()
			if puns.length>0
				num=rand(puns.length)
				client.send_message(puns[num])
			end
		when "!redeem"
			cooldown=5
			if arguments.length>0
				redeem(user,client,arguments[0])
			else
				externalIP=get_external_ip()
				client.send_message("#{user}: You can go to http://#{externalIP}/sorabot/ to view a list of redeemable items")
			end
		when "!quote"
			cooldown=30
			if arguments.length>0
				qnum=Integer(arguments[0])
				if curlJSON("GET","http://localhost:40000/Quote?id=#{qnum}",nil,nil,nil)["Status"]=="Success"
					client.send_message("Quote (#{qnum}): "+ret["Data"]["quote"])
				else
					client.send_message(user+", The quote you specified does not exist!")
				end
			else
				ret=curlJSON("GET","http://localhost:40000/Quote",nil,nil,nil)
				anum=ret["Data"]["number"]+1
				client.send_message("Quote (#{anum}): "+ret["Data"]["quote"])
			end
		#when "!current_song"
		#	cooldown=30
		#	songid=getsong()
		#	client.send_message(user+": The song last/currently playing can be found at: http://youtu.be/"+songid)
		when "!8ball"
			cooldown=30
			responses=["It is certain","It is decidedly so","Without a doubt","Yes definitely","You may rely on it","As I see it, yes","Most likely","Outlook good","Yes","Signs point to yes","Reply hazy try again","Ask again later","Better not tell you now","Cannot predict now","Concentrate and ask again","Don't count on it","My reply is no","My sources say no","Outlook not so good","Very doubtful"]
			num=rand(responses.length)
			client.send_message(user+": "+responses[num])
		when "!commands"
			cooldown=15
			externalIP=get_external_ip()
			client.send_message(user+": please go to http://#{externalIP}/sorabot/ to see a list of all commands supported on this channel.")
		when "!blanks"
			cooldown=10
			userblanks=curlJSON("GET","http://localhost:40000/Blanks/#{user}",nil,nil,nil)["Data"]
			client.send_message(user+" you have #{userblanks} SoraBlanks.")
		when "!tts"
			if not modded
       	client.Log("INFO","BOT_COMMAND","#{user} attempted to run !tts without authorization.")
				return true
			end
			enable_tts=curlJSON("GET","http://localhost:40000/TTS/Enabled",nil,nil,nil)["Data"]
			dat = enable_tts ? "enabled" : "disabled"
			if arguments.length==0
				client.send_message(user+", TTS is currently #{dat}.")
			else
				if arguments[0]=="enable"
					curlJSON("GET","http://localhost:40000/TTS/Enable",nil,nil,nil)
					client.send_message("TTS is now ENABLED.")
				elsif arguments[0]=="disable"
					curlJSON("GET","http://localhost:40000/TTS/Disable",nil,nil,nil)
					client.send_message("TTS is now DISABLED.")
				elsif arguments[0]=="ban"
					usr=arguments[1].downcase
					ret=curlJSON("GET","http://localhost:40000/TTS/Ban/#{usr}",nil,nil,nil)
					if ret["Status"]=="Failure"
						client.send_message(user+": #{usr} is already banned from TTS!")
					else
						client.send_message(user+": Banned #{usr} from TTS!")
					end
				elsif arguments[0]=="unban"
					usr=arguments[1].downcase
					ret=curlJSON("GET","http://localhost:40000/TTS/Unban/#{usr}",nil,nil,nil)
					if ret["Status"]=="Success"
						client.send_message(user+": Unbanned #{usr} from TTS!")
					else
						client.send_message(user+": #{usr} isn't banned from TTS!")
					end
				elsif arguments[0]=="speak"
					force_tts(arguments[1])
				end
			end
	
		
		when "!judge"
			return true #not implemented yet
			if not modded
				return true
			end
			if $poll_type == 0
				if arguments.length == 0
					client.send_message(user+": Cannot start a guessing session without specifying the poll type! (1 = Numeric)")
					return true
				elsif not is_number?(arguments[0])
					client.send_message(user+": Cannot create a guessing session with type "+arguments[0])
					return true
				end
				$poll_type = Float(arguments[0])
			else
				
			end
		when "!wr"
			cooldown=15
			if arguments.length > 0
				client.send_message(user+": "+client.getWorldRecord(arguments.join(" ")))
			else			
				client.send_message(user+": you must specify a category!")
			end
		when "!stream_status"
			cooldown=10
			client.send_message(if streamStatus() == nil then "Stream is not currently live!" else "Stream is currently live!" end)
		when "!reset_url_count"
			if not modded
				return true
			end
			usr=arguments[0].downcase
			curlJSON("PUT","http://localhost:40000/URLBanBreakers/#{usr}",nil,0,nil)
		when "!ignore"
			if not modded
				return true
			end
			if arguments[0].downcase=="emptysora_"
				client.ignore_user(user)
				client.send_message("User "+user+" has been ignored for abusing the bot!")
				return true #don't let users ban me
			end
			client.ignore_user(arguments[0].downcase)
			client.send_message("User "+arguments[0]+" will now be ignored!")
		when "!invite"
			if user != "emptysora_" #broadcaster only
				return true
			end
			invite=get_discord_invite()
			client.send_whisper(arguments[0].downcase,"Hey! You've been invited to EmptySora_'s Discord server! Open the discord invite link that follows to join! "+invite)
			client.send_message("An invitation to the discord server has been sent! :3")
		when "!unignore"
			if not modded
				return true
			end
			client.unignore_user(arguments[0].downcase)
			client.send_message("User "+arguments[0]+" will no longer be ignored!")
		when "!so"
			cooldown=60
			if arguments.length > 0
				client.send_message("Hey! You all should check out this awesome person: https://www.twitch.tv/"+arguments[0])
			end
		when "!day"
			cooldown=15
			msg=""
			case Time.now.in_time_zone('America/New_York').to_date.wday
				when 0
					msg="Today is Sunday! ResidentSleeper"
				when 1
					msg="Today is Monday! WutFace"
				when 2
					msg="Today is Tuesday! 4Head"
				when 3
					msg="Today is Wednesday! twitchRaid"
				when 4
					msg="Today is Friday-Eve! Kappa (Thursday)"
				when 5
					msg="Today is Friday! PogChamp"
				when 6
					msg="Today is Saturday! Kreygasm"
			end
			client.send_message(msg)
		when "!uptime"
			cooldown=10
			client.send_message(client.GetUptime())
		when "!refresh_moderators"
			if not modded
				return true
			end
			client.send_message("/mods");
			client.send_message("Refreshing moderator list...");
		else
			if not RunCustomCommand(user,message,command,arguments,client)
				cooloff=false
			end
	end
	if cooloff
		set_command_cooldown(command,cooldown)
	end
	return cooloff
end



def RunCustomCommand(user,message,command,arguments,client)
	begin
		com=curlJSON("GET","http://localhost:40000/CustomCommand/#{command}",nil,nil,nil)
		if com["Status"]=="Success"
			client.send_message(ParseCustomCommand(user,message,command,arguments,client,com["Data"]))
			return true
		else
			return false
		end
	rescue
	end
end
def ParseCustomCommand(user,message,command,args,client,res)
	mux={}
	mux["caller"]=user
	mux["param"]=args.join(" ")
	strm=streamStatus()
	if strm==nil or strm.class=="NilClass"
		strm={}		
		strm["game"]= "Not Playing"
		strm['viewers']= 0
		strm['video_height']= 0
		strm['average_fps']= 0
		strm['delay']= 0
		strm['created_at']= 'N/A'
		strm['is_playlist']= false
	end
	mux["game"]=strm["game"]
	mux["viewers"]=strm["viewers"]
	mux["fps"]=strm["average_fps"]
	mux["delay"]=strm["delay"]
	mux["start_time"]=strm["created_at"]

	mux["ip"]=get_external_ip()
	
	for i in 0..(args.length-1)
		mux["param"+(i+1)]=args[i]
	end
	ret=res
	mux.each do |key,value|
		ret=res.gsub("{"+key+"}","#{value}")
	end
	return ret
end
def AddCustomCommand(command_name,command_text)
	return curlJSON("PUT","http://localhost:40000/CustomCommand/#{command_name}",nil,command_text,nil)["StatusMessage"]=="Edit"
end

def DeleteCustomCommand(command_name)
	return curlJSON("DELETE","http://localhost:40000/CustomCommand/#{command_name}",nil,nil,nil)["Status"]=="Success"
end

$comcool={}
def is_command_on_cooldown(command)
	if $comcool.key?(command)
  	return $comcool[command] > Time.now.utc
	else
		return false
	end
end
def set_command_cooldown(command,cooldown)
	if cooldown==nil
		cooldown=60
	elsif cooldown<0
		cooldown=60
	end
	$comcool[command] = (Time.now + cooldown).utc
end

def get_users()
	url=URI("http://tmi.twitch.tv/group/user/emptysora_/chatters")
	results=Net::HTTP.get(url)
	return JSON.parse results
end

def get_external_ip()
	return "emptysora.com"
	url=URI("https://myexternalip.com/raw")
	results=Net::HTTP.get(url)
	return results.strip
end

def user_is_mod(username)
	chatters=get_users
	chatters=chatters["chatters"]
	if chatters["moderators"].include? username
		return true
	elsif chatters["staff"].include? username
		return true
	elsif chatters["admins"].include? username
		return true
	elsif chatters["global_mods"].include? username
		return true
	else
		return false
	end
end
def get_viewers()
	chatters=get_users
	chatters=chatters["chatters"]
	ret=[]
	ret+=chatters["moderators"]
	ret+=chatters["staff"]
	ret+=chatters["admins"]
	ret+=chatters["global_mods"]
	ret+=chatters["viewers"]
	return ret
end

$sema=Mutex.new
$csema=Mutex.new


def getsong()
	file=File.open($path+"/current_song.txt")
	contents=file.read
	file.close
	return contents
end




def get_puns()
	file=File.open($path+"/puns.txt")
	contents=file.read
	file.close
	return contents.sub("\r","").split("\n")
end
def get_blocked_urls()
	file=File.open($path+"/banned_urls.txt")
	contents=file.read
	file.close
	return contents.sub("\r","").split("\n")
end
def block_url(url)
	urls=get_blocked_urls()
	urls.push(url)
	File.write($path+"/banned_urls.txt",urls.join("\r\n"))
end


$tts_pipe=Pipe::Client.new("tts_pipe")
def do_tts(user,message)
	enable_tts=curlJSON("GET","http://localhost:40000/TTS/Enabled",nil,nil,nil)["Data"]
	if enable_tts
		if not is_tts_banned(user)
			$tts_pipe.write message
		end
	end
end
def force_tts(message)
	$tts_pipe.write message
end

def get_discord_invite()
	pipe=Pipe::Client.new("discord_bot_pipe",0x00000003) #duplexed
	pipe.write("GET_INVITE")
	ret=pipe.read()
	pipe.close()
	return ret
end

def scan_message(user,message)
	modded = user_is_mod(user)
	if modded
		return false
	end
	urls=get_blocked_urls
	words=message.split(" ")
	for i in 0..(words.length-1)
		word=words[i]
		for j in 0..(urls.length-1)
			url=urls[j]
			if word.starts_with?(url) or word.starts_with?("http://"+url) or word.starts_with?("https://"+url)
				return true
			end
		end
	end
	return false
end

def is_tts_banned(user)
	return curlJSON("GET","http://localhost:40000/TTS/Banned/#{user}",nil,nil,nil)["Data"]
end

def streamStatus()
	return curlJSONheaders("GET","https://api.twitch.tv/kraken/streams/emptysora_",{'Client-ID': IniFile.load($path+"/config.ini")["Client"]["ID"]})["stream"]
end

def curlJSONheaders(method,urlstring,headers)
	return curlJSON(method,urlstring,nil,nil,headers)
end
def curl(method,urlstring,query_params,post_data,headers)
	uri=URI(urlstring)
	if not query_params==nil
		uri.query=URI.encode_www_form(query_params)
	end
	req=nil
	case method
		when "GET"
			req=Net::HTTP::Get.new(uri)
		when "POST"
			req=Net::HTTP::Post.new(uri)
		when "PUT"
			req=Net::HTTP::Put.new(uri)
		when "DELETE"
			req=Net::HTTP::Delete.new(uri)
		when "OPTIONS"
			req=Net::HTTP::Options.new(uri)
		else
			return nil
	end
	if not headers==nil
		headers.each do |key,value|
			req[key]=value
		end
	end

	if not post_data==nil
		req.body=post_data
	end
	res=if urlstring.starts_with?("https://") then
		Net::HTTP.start(uri.hostname,uri.port, :use_ssl => uri.scheme == 'https'){|http| http.request(req) }
	else
		Net::HTTP.start(uri.hostname,uri.port){|http| http.request(req) }
	end
	#pp res
	#puts res.body
	return res.body
end
def curlJSON(m,u,q,p,h)
	return JSON.parse(curl(m,u,q,p,h))
end
def simpleCurlJSON(url,query)
	return curlJSON("GET",url,query,nil,nil)
end
def simpleCurl(url,query)
	return curl("GET",url,query,nil,nil)
end
def simplerCurlJSON(url)
	return simpleCurlJSON(url,nil)
end
def simplerCurl(url)
	return simpleCurl(url,nil)
end


def isTwitchOnline()
	return simplerCurl("http://irc.twitch.tv").strip=="404 page not found"
end
#next step: see how many times we don't get pinged, if it's over 5 minutes, check the ping, if failed, restartApp


#$poll_type=0
#def do_poll(user,msg)
#	polldata=get_persistent_value("poll_data")
#	if polldata==nil
#		polldata={}
#		set_persistent_value("poll_data",polldata)
#	end
#	case $poll_type
#		when 0 #No poll
#			return nil
#		when 1 #numeric
#			if is_number?(msg)
#				polldata[user]=Float(msg)
#				set_persistent_value("poll_data",polldata)
#			end
#		when 2 #time
#	end
#end

def is_number? string
	true if Float(string) rescue false
end

class MessageScanner

	def dispose()
		@terminate=true
	end

	def initialize()
		@terminate=false
		@message_queue = []
		@lockobj = Mutex.new
		Thread.new { # Scan Message Queue
			while true do
				if @terminate
					break
				end
				sleep(1)
				@lockobj.lock
				messagez = @message_queue.pop
				@lockobj.unlock
				while messagez do
					message=messagez[1]
					user=messagez[0]
					if scan_message(user,message)
						times=curlJSON("GET","http://localhost:40000/URLBanBreakers/Broken/#{user}",nil,nil,nil)["Data"]
						case times
							when 1
       					$client.Log("INFO","MESSAGE_SCANNER","#{user} sent a banned link: #{message} | first warning")
								$client.purge_user(user)
								$client.send_message("@"+user+" shortened urls are not allowed in this chat!")
							when 2
       					$client.Log("INFO","MESSAGE_SCANNER","#{user} sent a banned link: #{message} | second warning")
								$client.timeout_user(user,300)
								$client.send_message("@"+user+" shortened urls are not allowed in this chat, second warning!")
							when 3
       					$client.Log("INFO","MESSAGE_SCANNER","#{user} sent a banned link: #{message} | third warning")
								$client.timeout_user(user,1800)
								$client.send_message("@"+user+" shortened urls are not allowed in this chat, third warning!")
							when 4
       					$client.Log("INFO","MESSAGE_SCANNER","#{user} sent a banned link: #{message} | fourth/final warning")
								$client.timeout_user(user,3600)
								$client.send_message("@"+user+" shortened urls are not allowed in this chat, **FINAL WARNING**!")
							else
       					$client.Log("INFO","MESSAGE_SCANNER","#{user} sent a banned link: #{message} | banning user")
								$client.ban_user(user)
								$client.send_message("@"+user+" you have been banned for repeatedly posting shortened urls!")
						end
					end
					@lockobj.lock
					messagez = @message_queue.pop
					@lockobj.unlock
				end
			end
		}
	end
	def AddMessage(user,message)
		@lockobj.lock
		@message_queue << [user,message]
		@lockobj.unlock
	end
end
class FollowerWatcher
	def dispose()
		@terminate=true
	end
	def initialize()
		@terminate=false
		@FollowerCount=nil
		@today_followers=[]
		@filepath=""
	end
	def Start()
		Thread.new {
			begin
				puts "Started up Follower Watcher Thread"
				while true do
					if @terminate
						break
					end
					FollowerSearch()
					sleep(60) #minutely, we REALLY do not want to bork the API endpoints
				end
			rescue Exception => e
				puts "THREAD ERROR:"
				pp e
				puts e.backtrace.join("\n")
			end
		}
	end
	private
		def GetTotalFollowers()
			details=GetFollowerSet(0)
			return details["Total"]#number: total followers
		end
		def FollowerSearch()
			#timer event
			page=0
			newfollowtime=ParseTime("1997-01-01T00:00:00Z")
			while true do
				followers=GetFollowerSet(page)
				if followers["Total"]==0
					break
				end
				stop=false
				for i in 0..(followers["Length"] - 1)
					f=followers["Follows"][i]
					cd=ParseTime(f["created_at"])
					if IsFollowerNew(cd) and IsFollowerToday(cd)
						#found new follower
						if newfollowtime<cd
							newfollowtime=cd
						end
						if not HasFollowedToday(f["user"]["_id"])
							AddFollower(f["user"]["_id"])#Usernames may change, IDs WILL NOT
							$client.new_follower(f["user"]["display_name"])
						end
					elsif IsFollowerNew(cd) and (not IsFollowerToday(cd))
						#new follower but not today: do nothing
						if newfollowtime<cd
							newfollowtime=cd
						end
					elsif not IsFollowerNew(cd)
						stop=true
						break
					end
				end
				if stop
					break
				end
				page=page+1
			end
			if GetLastFollowTime()<newfollowtime
				SetLastFollowTime(newfollowtime)
			end
		end
		def GetFollowerSet(page)
			if (page==nil) or (page<0)
				page=0
			end
			data=curlJSON("GET","https://api.twitch.tv/kraken/channels/emptysora_/follows",{
				"limit": 100,
				"offset": page*100,
				"direction": "desc" #Descending so we don't double anyone
			},nil,{
				"Client-ID": IniFile.load($path+"/config.ini")["Client"]["ID"]
			})
			ret={}
			ret["Total"]=data["_total"]
			ret["Length"]=data["follows"].length
			ret["Follows"]=data["follows"]
			return ret
		end
		def ParseTime(timestring)
			#fix
			parts=timestring.split(".")
			timestringnew=parts[0]
			if not (timestring==timestringnew)
				timestring=timestringnew+"Z"
			end
			#end fix
			begin
				return DateTime.strptime(timestring,"%Y-%m-%dT%H:%M:%S.%6NZ")
			rescue
				begin
					return DateTime.strptime(timestring,"%Y-%m-%dT%H:%M:%SZ")
				rescue
					puts "Parsing a timestring: #{timestring}"
					return DateTime.strptime(timestring,"%Y-%m-%dT%H:%M:%SZ")
				end
			end
			#may need to do a "%6N" before "Z" not sure
		end
		def FormatTime(timeobj)
			return timeobj.strftime("%Y-%m-%dT%H:%M:%SZ")
		end
		def GetLastFollowTime()
			if not @lastfollowtime==nil
				return @lastfollowtime
			end
			lastfollow=curlJSON("GET","http://localhost:40000/LastFollow",nil,nil,nil)["Data"]
			@lastfollowtime=ParseTime(lastfollow)
			return @lastfollowtime
		end
		def SetLastFollowTime(time)
			lastfollow=FormatTime(time)
			curlJSON("PUT","http://localhost:40000/LastFollow",nil,lastfollow,nil)
			@lastfollowtime=time
		end
		def LoadTodayFile()
			xpath=GetTodayFile()
			txt=nil
			if not File.exist?(xpath)
				File.write(xpath,"")
			end
			txt=File.open(xpath).read
			txt.gsub!(/\r\n?/,"\n")
			arr=[]
			txt.each_line do |line|
				arr.push(line)
			end
			@today_followers=arr
			@filepath=xpath
		end
		def HasFollowedToday(username)
			fp=GetTodayFile()
			if not fp==@filepath
				LoadTodayFile()
			end
			return @today_followers.include? username
		end
		def AddFollower(username)
			if not HasFollowedToday(username)
				@today_followers.push(username)
				File.write(@filepath,@today_followers.join("\r\n"))
			end
		end
		def IsFollowerToday(followTime)
			today_start=ParseTime(DateTime.now.strftime("%Y-%m-%dT00:00:00Z"))
			today_end=ParseTime(DateTime.now.strftime("%Y-%m-%dT23:59:59Z"))
			if (followTime>=today_start) and (followTime<=today_end)
				return true
			else
				return false
			end
			#issue, what if time is between 23:59:59 and 00:00:00
			#issue, fractional seconds from twitch api may break the class
		end
		def IsFollowerNew(followTime)
			#New as in followed since last checked
			return GetLastFollowTime()<followTime
		end
		def GetTodayFile()
			return $path+"/Followers/["+DateTime.now.strftime("%Y%m%d")+"] Followers.txt"
		end
end
class SpeedRun #provider: SpeedRun.com
	def dispose()

	end
	def GetCurrentRecord(categoryName)
		status=streamStatus()
		if status==nil
			return "Unable to lookup World Record, currently offline!"
		end
		return GetWorldRecord(status["game"],categoryName)
	end
	def GetUptime()
		status=streamStatus()
		if status==nil
			return "Stream is not currently live!"
		end
		status=status["created_at"]
		dt=DateTime.strptime(status,"%Y-%m-%dT%H:%M:%SZ") #2001-01-02T23:45:51Z
		ep=Integer(dt.strftime("%s"))
		now=Integer(DateTime.now.strftime("%s"))
		diff=now-ep
		return "Current uptime is: "+formatTime(diff)
	end
	def GetWorldRecord(gameName,categoryName)
		sptext=GetSpeedrun(gameName,categoryName,1)
		if sptext==nil
			return "Game \""+gameName+"\" was not found!"
		end
		if sptext==false
			return "Category \""+categoryName+"\" was not found! Available Categories: "+getCategories(getGameID(gameName)[0])
		end
		if sptext==-1
			return "No runs found!"
		end
		return "World Record for "+gameName+" "+categoryName+" is "+sptext
	end
	def GetSpeedrun(gameName,categoryName,ranking)
		gid=getGameID(gameName)
		if gid==nil
			return nil
		end
		gname=gid[1]
		gid=gid[0]
		cid=getCategoryID(gid,categoryName)
		if cid==nil
			return false
		end
		lb=internalGetLeaderboard(gid,cid)
		for i in 0..(lb.length-1)
			run=lb[i]
			if run["place"]==ranking
				run=run["run"]
				player=getUserName(run["players"][0]["uri"])
				date=run["date"]
				time=formatTime(run["times"]["primary_t"])
				status=run["status"]["status"]
				verifier=getUserName("http://www.speedrun.com/api/v1/users/"+run["status"]["examiner"])
				link=""
				if run["videos"]["links"].length > 0
					link=run["videos"]["links"][0]["uri"]
				end

				result=time+" by "+player+" on "+date
				if status=="verified"
					result=result+" (verified by "+verifier+")"
				end
				if not link==""
					result=result+" ["+link+"]"
				end
				return result
			end
		end
		return -1
	end
	private
		def formatTime(fTime)
			dec=fTime%1
			whole=fTime-dec
			hours=0
			minutes=0
			seconds=whole
			milliseconds=dec
			if seconds > 60
				temp=seconds%60
				minutes=(seconds-temp)/60
				seconds=temp
			end
			if minutes > 60
				temp=minutes%60
				hours=(minutes-temp)/60
				minutes=temp
			end
			hours="#{hours}"
			minutes="#{minutes}"
			seconds="#{seconds}"
			milliseconds="#{milliseconds}"
			while milliseconds.length<3
				milliseconds="0"+milliseconds
			end
			while seconds.length<2
				seconds="0"+seconds
			end
			while minutes.length<2
				minutes="0"+minutes
			end
			while hours.length<2
				hours="0"+hours
			end
			return hours+":"+minutes+":"+seconds+"."+milliseconds
		end
		def getUserName(apiURL)
			return simplerCurlJSON(apiURL)["data"]["names"]["international"]
		end
		def getGameID(gameName)
			data=simpleCurlJSON("http://www.speedrun.com/api/v1/games",{name: gameName})["data"]
			if data.length==0
				return nil
			end
			return [data[0]["id"],data[0]["name"]]
		end
		def getCategoryID(gameID,categoryName)
			data=simplerCurlJSON("http://www.speedrun.com/api/v1/games/"+gameID+"/categories")["data"]
			if data.length==0
				return nil
			end
			for i in 0..(data.length-1)
				dat=data[i]
				if dat["name"]==categoryName
					return dat["id"]
				end
			end
			return nil
		end
		def getCategories(gameID)
			data=simplerCurlJSON("http://www.speedrun.com/api/v1/games/"+gameID+"/categories")["data"]
			if data.length==0
				return nil
			end
			ret=[]
			for i in 0..(data.length-1)
				ret.push(data[i]["name"])
			end
			return ret.join(", ")
		end
		def internalGetLeaderboard(gameID,categoryID)
			return simplerCurlJSON("http://www.speedrun.com/api/v1/leaderboards/"+gameID+"/category/"+categoryID)["data"]["runs"]
		end
end
def getClient(token)
	Twitch::Chat::Client.new(channel: $file["Account"]["Channels"],nickname:"SoraBot", password:"oauth:"+token, scanner: MessageScanner.new, speedruns: SpeedRun.new, follower_watcher: FollowerWatcher.new, oauth_token: token) do
		on(:connected) do
		 	#send_message 'This is a test message!'
        @follower_watcher.Start
		end
		#on(:disconnect) do
		# 	#send_message "The bot is now disconnecting!"
		#end
		on(:message) do |user, message|
			com=message.split[0]
			args=message.split
			args.shift
			if com.start_with?("!")
				#begin
					if not command_handle(user,message,com,args,$client)
						do_tts(user,message)
					end
				#rescue
					#ERROR
				#end
			else
				#do_poll(user,message)
				do_tts(user,message)
				@scanner.AddMessage(user,message)
			end
		end
		#on(:new_moderator) do |user|
		#	send_message "Hey everyone! Just a heads up, but #{user} has just joined our team of moderators! Let's see some HYPE!"
		#end
		#on(:remove_moderator) do |user|
		#	send_message "Hey everyone! Just a heads up, but #{user} will be leaving our team of moderators! No matter what, we still love you #{user}!"
		#end
		on(:new_follower) do |user|
			send_message "NEW FOLLOWER ALERT! <3 <3 <3 Thank you #{user} for following!"
		end
		on(:notice) do |message|
			if message.include?("The moderators of this room are:")
				modlist=message.split(":",2)[1].strip.split(", ")
				save_mods(modlist)
			end
		end
		on(:ping) do
			send_message "/mods" #refresh moderator list
		end
		#on(:raw) do |message|
		#	tmp=message.raw
		#	puts "Received Message: #{tmp}"
		#end
	end
end
$client = getClient(token)

def save_mods(mods)
	File.write($path+"/moderators.txt",mods.join("\r\n"));
end

while true do
	$client.run!
	while not isTwitchOnline() do
		puts "Twitch is still not back online, waiting 60 seconds"
		sleep(60)
	end
	puts "Restarting Twitch Chat Client"
	$client=getClient(token) #auto reconnect
end
