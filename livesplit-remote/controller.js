
//really, this should be "ajaj" (Asynchronous Javascript And JSON), not XML
function ajax(cmd)
{
	if(cmd=="reset")
	{
		if(!confirm("Are you sure you want to reset? This cannot be undone."))
			return;
	}
	let xhr=new XMLHttpRequest();
	let cover=document.querySelector("#cover");
	xhr.open("GET","/streaming/remote.php?c="+encodeURIComponent(cmd),true);
	xhr.onreadystatechange=function()
	{
		if(this.readyState==4&&this.status==200)
		{
			//#OldHat
			let json=JSON.parse(xhr.responseText);
			if(json.error)
				alert(json.message);
			window.setTimeout(function(){
				cover.style.display="none";
			},3000);
		}
	};
	cover.style.display="initial";
	xhr.send();
}
