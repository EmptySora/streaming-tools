require 'net/http'
require 'json'
require 'socket'
require 'pp'

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
		else
			return nil
	end
	if not headers==nil
		headers.each do |key,value|
			req[key]=value
		end
	end
	if method=="POST"
		req.set_form_data(post_data)
	end
	res=Net::HTTP.start(uri.hostname,uri.port){|http|
		http.request(req)
	}
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

module Twitch
	module API
		class TwitchUser
			#properties
			attr_reader ID
			attr_reader Biography
			attr_reader CreatedAt
			attr_reader DisplayName
			attr_reader Logo
			attr_reader UserName
			attr_reader UserType
			attr_reader UpdatedAt
			#Constructor
			def initialize(userID)
				user=simplerCurlJSON("https://api.twitch.tv/kraken/users/"+userID)
				@ID=userID
				@Biography=user["bio"]
				@CreatedAt=user["created_at"]
				@DisplayName=user["display_name"]
				@Logo=user["logo"]
				@UserName=user["name"]
				@UserType=user["type"]
				@UpdatedAt=user["updated_at"]
			end
			#static TwitchUser.FromUserName
			def self.FromUsername(username)
				user=simpleCurlJSON("https://api.twitch.tv/kraken/users",{login: username})
				if user["_total"] > 0
					return TwitchUser.new(user["users"][0]["_id"])
				end
			end
			def GetFollowedChannels()
				user=@ID
				data=simplerCurlJSON("https://api.twitch.tv/kraken/users/#{user}/follows/channels?limit=100")
				ret={Count: data["_total"],Items:[]}
				
			end
		end
		class TwitchFollow
			attr_reader CreatedAt
			attr_reader Notifications
			attr_reader Channel
			def initialize(object)
				@CreatedAt=object["created_at"]
				@Notifications=object["notifications"]
				@Channel=TwitchChannel.new(object["channel"]["_id"])
			end
		end
		class TwitchChannel

		end
	end
end
