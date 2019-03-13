-- !addquote
function Command(user, message, command, arguments, modded)
	if not modded
		local qnum = client.CurlJSON("PUT","http://localhost:40000/Quote/Recommend",nil,arguments:join(" ") + " (Submitted by " + user + ")",nil):This("Data"):This("number"):GetValue() --Convert to object, should work
		client:SendMessage(user + ", added quote ("+qnum+") for recommendation!")
		return true
	end
	local qnum = client.CurlJSON("PUT","http://localhost:40000/Quote",nil,arguments:join(" "),nil):This("Data"):This("number"):GetValue() --Convert to object, should work
	client:SendMessage(user + ", added quote ("+qnum+")!")
end
