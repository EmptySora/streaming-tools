<?php
if (!isset($_GET["module"])) {
	include("main.php");
} else {
	switch ($_GET["module"]) {
	case "list_titles":
		header("Content-Type: application/json");
		echo titles();
		break;
	case "add_title":
		add_title();
		break;
	default:
	    outputErrorPage(400); //bad request
	    break;
	}
}


const MYSQLI_PREPARE_PHASE_PREPARE=0;
const MYSQLI_PREPARE_PHASE_BIND=1;
const MYSQLI_PREPARE_PHASE_EXECUTE=2;
/*
Usage:
mysqli_safe_query($con,"INSERT INTO ? WHERE ?=? SET ?=?",$error,"sspsp",$tb,$indCol,$indVal,$col,$val);
mysqli_prepared_query($con,"INSERT INTO table WHERE key=? SET bkey=?",$error,$indVal,$val);
*/


function mysqli_sanitize_query_bit($bit)
{
    return preg_replace('/[^0-9a-zA-Z\x24_\x80-\xFF]/','',"$bit");
}
function mysqli_safe_query($con,$sql,&$err,$tstring)
{
    $sqlarr=explode("?",$sql);
    $glue=array();
    $bargs=func_get_args();
    $cargs=func_num_args();
    for($i=1;$i<count($sqlarr);$i++)
        $glue[]="?";
//p=parameter (pass as parameter)
//s=sanitize (manual sanitization)
    for($i=0;$i<strlen($tstring);$i++)
        if($tstring[$i]=="s")
            $glue[$i]=mysqli_sanitize_query_bit($bargs[$i+4]);
    
    $sqltemp="";
    for($i=1;$i<=(count($sqlarr)+count($glue));$i++)
    {
        if(($i%2)==0)
        {
            //glue
            $j=($i/2)-1;
            $sqltemp.=$glue[$j];
        }
        else
        {
            //parts
            $j=(($i+1)/2)-1;
            $sqltemp.=$sqlarr[$j];
        }
    }
    $args=array($con,$sqltemp,&$err);
    for($i=0;$i<count($glue);$i++)
        if($tstring[$i]=="p")
            $args[]=$bargs[$i+4];
    
    return call_user_func_array("mysqli_prepared_query",$args);//mysqli_prepared_query()
}
function mysqli_prepared_query($con,$sql,&$err)
{
    //eg: SELECT * FROM test WHERE id=?, subid=?
    $args=array();
    $bargs=func_get_args();
    $cargs=func_num_args();
    for($i=3;$i<$cargs;$i++)
        $args[]=$bargs[$i];
    if(!($res=$con->prepare($sql)))
    {
        $err=array(
            'phase'=>MYSQLI_PREPARE_PHASE_PREPARE,
            'errno'=>$con->errno,
            'error'=>$con->error,
            'arguments'=>array(
                'con'=>$con,
                'sql'=>$sql,
                'args'=>$args
            )
        );
        return false;
    }
    $tstr='';
    for($i=0;$i<count($args);$i++)
        $tstr.='s';
    $rfarr=array($tstr);
    foreach($args as $k=>$v)
        $rfarr[]=&$args[$k];
    $ref=new ReflectionClass('mysqli_stmt');
    $method=$ref->getMethod('bind_param');
    if(!($method->invokeArgs($res,$rfarr)))
    {
        $err=array(
            'phase'=>MYSQLI_PREPARE_PHASE_BIND,
            'errno'=>$res->errno,
            'error'=>$res->error,
            'arguments'=>array(
                'con'=>$con,
                'sql'=>$sql,
                'args'=>$args
            )
        );
        $res->close();
        return false;
    }
    if(!($res->execute()))
    {
        $err=array(
            'phase'=>MYSQLI_PREPARE_PHASE_EXECUTE,
            'errno'=>$res->errno,
            'error'=>$res->error,
            'arguments'=>array(
                'con'=>$con,
                'sql'=>$sql,
                'args'=>$args
            )
        );
        $res->close();
        return false;
    }
    $ret=$res->get_result();
    return ($ret)?$ret:true;
}






function isInteger($input) {
    return ctype_digit(strval($input));
}

function do_sql($sql) {
    //performs a query, and fetches the result rows as an array of associative arrays
	$con = new mysqli("YOUR SQL SERVER ADDRESS", "USERNAME", "PASSWORD", "streaming_db");
	$res = $con->query($sql);
	$ret = array();
	while ($row = $res->fetch_assoc()) {
		$ret[] = $row;
	}
	$con->close();
	return $ret;
}

function add_title() {
	$game = $_POST["game"];
	$title = $_POST["title"];
	$tags = $_POST["tags"];
	
	$sql = "INSERT INTO stream_titles SET Game=?, Title=?, Tags=?";
	$con = new mysqli("YOUR SQL SERVER ADDRESS", "USERNAME", "PASSWORD", "streaming_db");
	mysqli_safe_query($con, $sql, $error, "ppp", $game, $title, $tags);
	$con->close();
	if (isset($error)) {
		var_dump($error);
	}
}

function titles() {
	$rows = do_sql("SELECT * FROM stream_titles");
	$ret = array();
	foreach ($rows as $row) {
		$ret[] = array(
		    "game" => $row["Game"],
		    "title" => $row["Title"],
		    "id" => $row["ID"],
		    "tags" => $row["Tags"],
		    "stamp" => $row["Creation"]
		);
	}
	return json_encode(array("data" => $ret));
}

?>
