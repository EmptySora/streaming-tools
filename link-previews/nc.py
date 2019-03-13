# loads a request via the GET method and returns the headers and metadata
def HeadRequest(secure,host,request):
  #conn = None
  #if secure:
  #  conn = http.client.HTTPSConnection(host,443) #open a HTTPS connection via port 443
  #else:
  #  conn = http.client.HTTPConnection(host,80) #open a regular http connection via port 80
  #conn.putrequest('GET',request) #tell server we are doing a HEAD request for resource "request"
  #conn.endheaders() #end the headers
  #response = conn.getresponse()
  return [host]#,response.getheaders(), ParseHtml(response.read())]


class HeadParser(HTMLParser):
  #we need to catch link and meta
  in_heading = 0
  Metadata = []
  def handle_starttag(self,tag,attributes):#use for attr in attrs and square brackets to access key and value
    if self.in_heading > 0:
      if tag == "meta":
        self.Metadata.append(HTMLMetadata(attributes))
      elif tag == "link":
        self.Metadata.append(HTMLLink(attributes))
    elif tag == "head":
      self.in_heading = self.in_heading + 1
  
  def handle_endtag(self,tag):
    if tag == "head":
      self.in_heading = self.in_heading - 1

class HTMLLink:
  is_link = True
  As = None
  CrossOrigin = None
  HREF = None
  HREF_Lang = None
  Integrity = None
  Media = None
  ReferrerPolicy = None
  Relationship = None #REL
  Sizes = None
  Title = None
  Type = None
  def __init__(self,attributes):
    for attribute in attributes:
      name = attribute[0]
      value = attribute[1]
      if name == "as":
        self.As = value
      elif name == "crosssorigin":
        self.CrossOrigin = value
      elif name == "href":
        self.HREF = value
      elif name == "hreflang":
        self.HREF_Lang = value
      elif name == "integrity":
        self.Integrity = value
      elif name == "media":
        self.Media = value
      elif name == "referrerpolicy":
        self.ReferrerPolicy = value
      elif name == "rel":
        self.Relationship = value
      elif name == "sizes":
        self.Sizes = value
      elif name == "title":
        self.Title = value
      elif name == "type":
        self.Type = value


class HTMLMetadata:
  is_link = False
  CharacterSet = None
  Content = None
  HTTP_Equivalent = None
  Name = None
  Property = None
  
  def __init__(self,attributes):
    for attribute in attributes:
      name = attribute[0]
      value = attribute[1]
      if name == "charset":
        self.CharacterSet = value
      elif name == "content":
        self.Content = value
      elif name == "http-equiv":
        self.HTTP_Equivalent = value
      elif name == "name":
        self.Name = value
      elif name == "property":
        self.Property = value

def ParseHtml(html):
  parser = HTMLParser()
  parser.feed(html)
  return parser.Metadata
