-- !quote
function Command(user, message, command, arguments, modded)
	-- Lua Command Template
	if arguments.Length > 0
		local qnum = tonumber(arguments[0])
		if client.CurlJSON("GET","http://localhost:40000/Quote?id="+qnum,nil,nil,nil):This("Status"):GetValue()=="Success" --Convert to object, should work
			client:SendMessage("Quote ("+qnum+"): "+)
end
cooldown = 30
