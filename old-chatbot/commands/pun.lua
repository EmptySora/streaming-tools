-- !pun
function Command(user, message, command, arguments, modded)
	local puns=client.GetPuns()
	if puns.Length>0
		local num=rand.Next(puns.Length)
		client:SendMessage(puns[num])
	end
end
