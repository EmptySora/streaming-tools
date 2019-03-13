-- !delquote
function Command(user, message, command, arguments, modded)
	if not modded
		client:Log("INFO","BOT_COMMAND",user+" attempted to run !delquote without authorization.")
		return true
	end
	local qnum=tonumber(arguments[0])
	if client.CurlJSON("DELETE","http://localhost:40000/Quote?id="+qnum,nil,nil,nil):This("Status"):GetValue()=="Success" --Convert to object, should work
		client:SendMessage(user+", Quote " + qnum + " has been removed.")
	else
    client:SendMessage(user+", Quote " + qnum + " does not exist.")
	end
end
