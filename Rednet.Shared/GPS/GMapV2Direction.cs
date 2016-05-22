using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Threading.Tasks;
#if !PCL
using System.Xml;
#endif
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
//using Exception = Java.Lang.Exception;

namespace Rednet.Shared.GPS
{
    public class GMapV2Direction
    {

        public GMapV2Direction()
        {
        }

        public RootObject GetRootObject(Location start, Location end, GMapV2DirectionMode mode)
        {
            var _data = GetDirectionsFromServer(start, end, mode, GMapV2DirectionReturnType.Json);
            var _ret = JsonConvert.DeserializeObject<RootObject>(_data);
            return _ret;
        }

        private string GetDirectionsFromServer(Location start, Location end, GMapV2DirectionMode mode, GMapV2DirectionReturnType typeServer)
        {
            string _mode = "";
            string _type = "";

            switch (mode)
            {
                case GMapV2DirectionMode.Driving:
                    _mode = "driving";
                    break;
                case GMapV2DirectionMode.Walking:
                    _mode = "walking";
                    break;
            }

            switch (typeServer)
            {
                case GMapV2DirectionReturnType.Xml:
                    _type = "xml";
                    break;
                case GMapV2DirectionReturnType.Json:
                    _type = "json";
                    break;
            }

            var _format = new CultureInfo("en-us").NumberFormat;

            string _url = string.Format("http://maps.googleapis.com/maps/api/directions/{0}?origin={1},{2}&destination={3},{4}&sensor=false&units=metric&mode={5}", _type, start.Latitude.ToString(_format), start.Longitude.ToString(_format), end.Latitude.ToString(_format), end.Longitude.ToString(_format), _mode);

            return this.GetStringDataFromServer(_url);

        }

        private string GetStringDataFromServer(string _url)
        {
#if !PCL
            try
            {
                WebRequest _webRequest = WebRequest.Create(_url);
                _webRequest.Timeout = 15000;
                _webRequest.Method = "POST";
                _webRequest.ContentType = "application/x-www-form-urlencoded";
                //_webRequest.ContentType = "application/json";

                var _teste = _webRequest.GetRequestStream();
                //_webRequest.ContentLength = 300000000;
                
                WebResponse _webResponse = _webRequest.GetResponse();
                string _data = "";

                using (var _stream = new System.IO.StreamReader(_webResponse.GetResponseStream()))
                {
                    _data = _stream.ReadToEnd();
                    _stream.Close();
                }

                return _data;
            }
            catch (System.Exception e)
            {
                return "";
            }
#else
            return "";
#endif
        }

        public List<Location> GetRoteDirections(Location start, Location end, GMapV2DirectionMode mode)
        {

            var _ret = new List<Location>();
            var _data = this.GetDirectionsFromServer(start, end, mode, GMapV2DirectionReturnType.Json);

            if (!_data.Equals(""))
            {
                JObject _result = JObject.Parse(_data);

                var _status = _result["status"];
                var _routes = JArray.Parse(_result["routes"].ToString());

                if (_status.ToString().Equals("OK"))
                {
                    var _legs = JArray.Parse(_routes[0]["legs"].ToString());
                    var _steps = JArray.Parse(_legs[0]["steps"].ToString());

                    foreach (var _step in _steps)
                    {
                        _ret.Add(this.GetLatLng(JObject.Parse(_step["start_location"].ToString())));
                        var _polyline = JObject.Parse(_step["polyline"].ToString());
                        _ret.AddRange(this.DecodePolylinePoints(_polyline["points"].ToString()));
                        _ret.Add(this.GetLatLng(JObject.Parse(_step["end_location"].ToString())));
                    }
                }
            }

            return _ret;

        }

        private Location GetLatLng(JObject data)
        {
            double _lat = data["lat"].ToString().ToDouble(true);
            double _lng = data["lng"].ToString().ToDouble(true);
            return new Location(_lat, _lng);
        }

        public async Task<Location> GetLocationFromAddressAsync(string address)
        {
            return await Task.Run(() => this.GetLocationFromAddress(address));
        }

        public Location GetLocationFromAddress(string address)
        {
            string _url = string.Format("https://maps.googleapis.com/maps/api/geocode/json?address={0}&sensor=false", address);

            var _data = this.GetStringDataFromServer(_url);

            JObject _result = JObject.Parse(_data);

            var _status = _result["status"];
            var _dados = JArray.Parse(_result["results"].ToString());

            if (_status.ToString().Equals("OK"))
            {
                var _geometry = JObject.Parse(_dados[0]["geometry"].ToString());

                var _location = JObject.Parse(_geometry["location"].ToString());

                return this.GetLatLng(_location);

            }
            else
                return null;
        }

#if !PCL
        public XmlDocument GetXmlDocument(Location start, Location end, GMapV2DirectionMode mode)
        {
            XmlDocument doc = new XmlDocument(); 
            doc.LoadXml(this.GetDirectionsFromServer(start, end, mode, GMapV2DirectionReturnType.Xml));
            return doc;
        }

        public string GetDurationText(XmlDocument doc)
        {
            try
            {

                XmlNodeList nl1 = doc.GetElementsByTagName("duration");
                XmlNode node1 = nl1.Item(0);
                XmlNodeList nl2 = node1.ChildNodes;
                XmlNode node2 = nl2.Item(GetNodeIndex(nl2, "text"));
                return node2.InnerText;
            }
            catch (Exception e)
            {
                return "0";
            }
        }

        public int GetDurationValue(XmlDocument doc)
        {
            try
            {
                XmlNodeList nl1 = doc.GetElementsByTagName("duration");
                XmlNode node1 = nl1.Item(0);
                XmlNodeList nl2 = node1.ChildNodes;
                XmlNode node2 = nl2.Item(GetNodeIndex(nl2, "value"));
                return int.Parse(node2.InnerText);
            }
            catch (Exception e)
            {
                return -1;
            }
        }

        public string GetDistanceText(XmlDocument doc)
        {

            try
            {
                XmlNodeList nl1;
                nl1 = doc.GetElementsByTagName("distance");

                XmlNode node1 = nl1.Item(nl1.Count - 1);
                XmlNodeList nl2 = null;
                nl2 = node1.ChildNodes;
                XmlNode node2 = nl2.Item(GetNodeIndex(nl2, "value"));
                return node2.InnerText;
            }
            catch (Exception e)
            {
                return "-1";
            }

        }

        public int GetDistanceValue(XmlDocument doc)
        {
            try
            {
                XmlNodeList nl1 = doc.GetElementsByTagName("distance");
                XmlNode node1 = null;
                node1 = nl1.Item(nl1.Count - 1);
                XmlNodeList nl2 = node1.ChildNodes;
                XmlNode node2 = nl2.Item(GetNodeIndex(nl2, "value"));
                return int.Parse(node2.InnerText);
            }
            catch (Exception e)
            {
                return -1;
            }
        }

        public string GetStartAddress(XmlDocument doc)
        {
            try
            {
                XmlNodeList nl1 = doc.GetElementsByTagName("start_address");
                XmlNode node1 = nl1.Item(0);
                return node1.InnerText;
            }
            catch (Exception e)
            {
                return "-1";
            }

        }

        public string GetEndAddress(XmlDocument doc)
        {
            try
            {
                XmlNodeList nl1 = doc.GetElementsByTagName("end_address");
                XmlNode node1 = nl1.Item(0);
                return node1.InnerText;
            }
            catch (Exception e)
            {
                return "-1";
            }
        }

        public string GetCopyRights(XmlDocument doc)
        {
            try
            {
                XmlNodeList nl1 = doc.GetElementsByTagName("copyrights");
                XmlNode node1 = nl1.Item(0);
                return node1.InnerText;
            }
            catch (Exception e)
            {
                return "-1";
            }

        }

        public List<Location> GetDirection(XmlDocument doc)
        {
            XmlNodeList nl1, nl2, nl3;
            List<Location> listGeopoints = new List<Location>();
            nl1 = doc.GetElementsByTagName("step");
            if (nl1.Count > 0)
            {
                for (int i = 0; i < nl1.Count; i++)
                {
                    XmlNode node1 = nl1.Item(i);
                    nl2 = node1.ChildNodes;

                    XmlNode locationNode = nl2
                        .Item(GetNodeIndex(nl2, "start_location"));
                    nl3 = locationNode.ChildNodes;
                    XmlNode latNode = nl3.Item(GetNodeIndex(nl3, "lat"));
                    double lat = double.Parse(latNode.InnerText);
                    XmlNode lngNode = nl3.Item(GetNodeIndex(nl3, "lng"));
                    double lng = double.Parse(lngNode.InnerText);
                    listGeopoints.Add(new Location(lat, lng));

                    locationNode = nl2.Item(GetNodeIndex(nl2, "polyline"));
                    nl3 = locationNode.ChildNodes;
                    latNode = nl3.Item(GetNodeIndex(nl3, "points"));
                    List<Location> arr = DecodePoly(latNode.InnerText);
                    for (int j = 0; j < arr.Count; j++)
                    {
                        listGeopoints.Add(new Location(arr[j].Latitude, arr[j].Longitude));
                    }

                    locationNode = nl2.Item(GetNodeIndex(nl2, "end_location"));
                    nl3 = locationNode.ChildNodes;
                    latNode = nl3.Item(GetNodeIndex(nl3, "lat"));
                    lat = double.Parse(latNode.InnerText);
                    lngNode = nl3.Item(GetNodeIndex(nl3, "lng"));
                    lng = double.Parse(lngNode.InnerText);
                    listGeopoints.Add(new Location(lat, lng));
                }
            }

            return listGeopoints;
        }

        private int GetNodeIndex(XmlNodeList nl, string nodename)
        {
            for (int i = 0; i < nl.Count; i++)
            {
                if (nl.Item(i).Name.Equals(nodename))
                    return i;
            }
            return -1;
        }
#endif

        private List<Location> DecodePolylinePoints(string encodedPoints)
        {
            if (encodedPoints == null || encodedPoints == "") return null;
            List<Location> poly = new List<Location>();
            char[] polylinechars = encodedPoints.ToCharArray();
            int index = 0;

            int currentLat = 0;
            int currentLng = 0;
            int next5bits;
            int sum;
            int shifter;

            try
            {
                while (index < polylinechars.Length)
                {
                    // calculate next latitude
                    sum = 0;
                    shifter = 0;
                    do
                    {
                        next5bits = (int)polylinechars[index++] - 63;
                        sum |= (next5bits & 31) << shifter;
                        shifter += 5;
                    } while (next5bits >= 32 && index < polylinechars.Length);

                    if (index >= polylinechars.Length)
                        break;

                    currentLat += (sum & 1) == 1 ? ~(sum >> 1) : (sum >> 1);

                    //calculate next longitude
                    sum = 0;
                    shifter = 0;
                    do
                    {
                        next5bits = (int)polylinechars[index++] - 63;
                        sum |= (next5bits & 31) << shifter;
                        shifter += 5;
                    } while (next5bits >= 32 && index < polylinechars.Length);

                    if (index >= polylinechars.Length && next5bits >= 32)
                        break;

                    currentLng += (sum & 1) == 1 ? ~(sum >> 1) : (sum >> 1);
                    var p = new Location((Convert.ToDouble(currentLat)/100000.0), (Convert.ToDouble(currentLng)/100000.0));
                    poly.Add(p);
                }
            }
            catch (Exception ex)
            {
                // logo it
            }
            return poly;
        }

        private List<Location> DecodePoly(string encoded)
        {
            List<Location> poly = new List<Location>();
            int index = 0, len = encoded.Length;
            int lat = 0, lng = 0;
            while (index < len)
            {
                int b, shift = 0, result = 0;
                do
                {
                    b = encoded.ToCharArray()[index++] - 63;
                    result |= (b & 0x1f) << shift;
                    shift += 5;
                } while (b >= 0x20);
                int dlat = ((result & 1) != 0 ? ~(result >> 1) : (result >> 1));
                lat += dlat;
                shift = 0;
                result = 0;
                do
                {
                    b = encoded.ToCharArray()[index++] - 63;
                    result |= (b & 0x1f) << shift;
                    shift += 5;
                } while (b >= 0x20);
                int dlng = ((result & 1) != 0 ? ~(result >> 1) : (result >> 1));
                lng += dlng;

                Location position = new Location((double)lat / 1E5, (double)lng / 1E5);
                poly.Add(position);
            }
            return poly;
        }
    }
}