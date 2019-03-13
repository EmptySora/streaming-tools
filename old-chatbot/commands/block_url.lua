-- !block_url
function Command(user, message, command, arguments, modded)
	if not modded
		client:Log("INFO","BOT_COMMAND",user+ " attempted to run !block_url without authorization.")
		return true
	end
	client.BlockURL(arguments[1])
end
