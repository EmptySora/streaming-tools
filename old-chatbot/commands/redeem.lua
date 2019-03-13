-- !redeem
function Command(user, message, command, arguments, modded)
	-- Lua Command Template
	if arguments.Length > 0
		client.Redeem(user,arguments[0])
	else
		local externalIP=client.GetExternalIP()
		client:SendMessage(user+": You can go to https://"+externalIP+"/sorabot/ to view a list of redeemable items")
	end
end
cooldown = 5
