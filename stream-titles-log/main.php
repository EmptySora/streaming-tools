<html>
	<head>
		<meta charset="utf-8"/>
		<title>Stream Resources</title>
		<script type="text/javascript" language="javascript" src="main.js"></script>
		<link rel="stylesheet" type="text/css" href="main.css"/>
	</head>
	<body>
	    <div id="main-content"></div>
	    <div id="templates">
	        <div id="template_tags">
	            <table x-class="taglist-window popup">
                    <thead>
                        <tr>
                            <th>Tags</th>
                        </tr>
                    </thead>
                    <tfoot>
                        <tr>
                            <td style="border-bottom:0px solid transparent;"  x-id="tTaglist">
                                <ul></ul>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <input type="button" value="Back" x-id="tBack" title="Back to Main Area"/>
                            </td>
                        </tr>
                    </tfoot>
                </table>
	        </div>
	        <div id="template_main">
	            <center>
                    <div x-id="content">
                        <div x-id="titles" x-class="section">
                            <table x-id="titlesTable">
                                <thead>
                                    <tr>
                                        <th colspan="3">Stream Titles</th>
                                    </tr>
                                    <tr>
                                        <th>Title</th>
                                        <th>Tags</th>
                                        <th>Timestamp</th>
                                    </tr>
                                </thead>
                                <tbody></tbody>
                                <tfoot>
                                    <tr>
                                        <td colspan="3" x-class="addrow" x-id="tAdd">
                                            Add Stream Title
                                        </td>
                                    </tr>
                                </tfoot>
                            </table>
                        </div>
                        <p/>
                        <br/>
                        <!--Levels section here-->
                    </div>
                </center>
                <div x-id="cover"></div>
                <div x-id="modal" style="display:none;">
                    <center></center>
                </div>
	        </div>
	        <div id="template_error">
                <table x-class="status-window popup">
                    <thead>
                        <tr>
                            <th>Error</th>
                        </tr>
                    </thead>
                    <tfoot>
                        <tr>
                            <td style="border-bottom:0px solid transparent;">
                                Your request completed with an unsatisfactory response!
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <input type="button" value="Back" x-id="eBack" title="Back to Main Area"/>
                            </td>
                        </tr>
                    </tfoot>
                </table>
	        </div>
	        <div id="template_success">
	            <table x-class="status-window popup">
                    <thead>
                        <tr>
                            <th>Success</th>
                        </tr>
                    </thead>
                    <tfoot>
                        <tr>
                            <td style="border-bottom:0px solid transparent;">
                                Your request has been successfully completed!
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <input type="button" value="Back" x-id="sBack" title="Back to Main Area"/>
                            </td>
                        </tr>
                    </tfoot>
                </table>
	        </div>
	        <div id="template_pending">
	            <table x-class="status-window popup">
                    <thead>
                        <tr>
                            <th>Request Sent</th>
                        </tr>
                    </thead>
                    <tfoot>
                        <tr>
                            <td style="background-color:white;">
                                Your request has successfully been<br/>
                                sent to the server. Awaiting Response...
                            </td>
                        </tr>
                    </tfoot>
                </table>
            </div>
	        <div id="template_addtitleform">
	            <form action="javascript:void(0)" formtype="add_title" x-id="addTitleForm" x-class="popup">
                    <table>
                        <thead>
                            <tr>
                                <th colspan="2">Add Stream Title</th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr>
                                <td>Game</td>
                                <td>
                                    <input x-name="game" type="text" placeholder="[Game Name]" required="required"/>
                                </td>
                            </tr>
                            <tr>
                                <td>Title</td>
                                <td>
                                    <input x-name="title" type="text" placeholder="[Stream Title]" required="required" maxlength="140"/>
                                </td>
                            </tr>
                            <tr>
                                <td>Tags</td>
                                <td>
                                    <input x-name="tags" type="text" placeholder="[Tags]" maxlength="65535" title="Comma deliminated list of tags"/>
                                </td>
                            </tr>
                        </tbody>
                        <tfoot>
                            <tr>
                                <td colspan="2">
                                    <center>
                                        <input x-name="submit" type="submit" value="Submit"/>
                                        &nbsp;&bullet;&nbsp;
                                        <input x-name="reset" type="reset" value="Reset"/>
                                        &nbsp;&bullet;&nbsp;
                                        <input x-name="cancel" type="button" value="Cancel" x-id="atfCancel"/>
                                    </center>
                                </td>
                            </tr>
                        </tfoot>
                    </table>
                </form>
	        </div>
	    </div>
	</body>
</html>
