<?php
//error_reporting(E_ALL);
header("Content-Type: application/json");
function sendCommand($cmdText)
{
	$cmdText.="\r\n";
	$address='192.168.0.128';//'192.168.0.128';//'169.254.159.209';
	$port=21212; //CHANGE THIS TO THE PORT SPECIFIED IN LIVESPLIT
	if(($sock=socket_create(AF_INET,SOCK_STREAM,SOL_TCP))===false)
		response(array(
			'error'=>true,
			'message'=>'Failed to open socket: '.socket_strerror(socket_last_error($sock)).' ('.socket_last_error($sock).')'
		));
	
	if(($result=socket_connect($sock,$address,$port))===false)
		response(array(
			'error'=>true,
			'message'=>'Failed to connect socket: '.socket_strerror(socket_last_error($sock)).' ('.socket_last_error($sock).')'
		));
	socket_write($sock,$cmdText,strlen($cmdText));
	/*
	$ret="";
	while($out=socket_read($sock,2048))
		$ret.=$out;
	socket_close($sock);
	*/
	response(array(
		'error'=>false,
		'message'=>$ret
	));
}

if(isset($_GET['c']))
{
	switch(strtolower($_GET['c']))
	{
		case 'start':
		case 'split':
			sendCommand("startorsplit");
			break;
		case 'undo':
			sendCommand("unsplit");
			break;
		case 'skip':
			sendCommand("skipsplit");
			break;
		case 'pause':
			sendCommand("pause");
			break;
		case 'resume':
			sendCommand("resume");
			break;
		case 'reset':
			sendCommand("reset");
			break;
	}
	response(array(
		'error'=>true,
		'message'=>'Unknown command: '.$_GET['c']
	));
	
}
response(array(
	'error'=>true,
	'message'=>'Invalid Request'
));
function response($r)
{
	echo json_encode($r);
	die;
}
?>
