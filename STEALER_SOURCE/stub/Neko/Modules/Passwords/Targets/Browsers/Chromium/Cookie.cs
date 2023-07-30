using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Globalization;
using JsonToNetscape;

namespace JsonToNetscape
{
    public class TimeUtils
    {
        protected static readonly DateTime UtcLinuxStartEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static DateTime FromSecondsSinceEpoch(double time)
        {
            return UtcLinuxStartEpoch.AddSeconds(time).ToLocalTime();
        }

        public static double ToSecondsSinceEpoch(DateTime time)
        {
            return (time.ToUniversalTime() - UtcLinuxStartEpoch).TotalSeconds;
        }
    }
}

namespace Plugin
{
    public class Cookie
    {
        private string _forDebug;
        private string _domain;
        private int _expirationDate;
        private bool _hostOnly;
        private bool _httpOnly;
        private string _name;
        private string _path;
        private string _sameSite;
        private bool _secure;
        private bool _session;
        private string _storeId;
        private string _value;
        //private int _id;

        // Getters and setters
        public string forDebug
        {
            get { return _forDebug; }
            set { _forDebug = value; }
        }
        public string Domain
        {
            get { return _domain; }
            set { _domain = value; }
        }
        public int ExpirationDate
        {
            get { return _expirationDate; }
            set { _expirationDate = value; }
        }
        public bool HostOnly
        {
            get { return _hostOnly; }
            set { _hostOnly = value; }
        }
        public bool HttpOnly
        {
            get { return _httpOnly; }
            set { _httpOnly = value; }
        }
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        public string Path
        {
            get { return _path; }
            set { _path = value; }
        }
        public string SameSite
        {
            get { return _sameSite; }
            set { _sameSite = value; }
        }
        public bool Secure
        {
            get { return _secure; }
            set { _secure = value; }
        }
        public bool Session
        {
            get { return _session; }
            set { _session = value; }
        }
        public string StoreId
        {
            get { return _storeId; }
            set { _storeId = value; }
        }
        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }
        //public int Id
        //{
        //    get { return _id; }
        //    set { _id = value; }
        //}

        public void SetSameSiteCookie(string val)
        {
            switch (val)
            {
                case "-1":
                    this.SameSite = "unspecified";
                    break;
                case "0":
                    this.SameSite = "no_restriction";
                    break;
                case "1":
                    this.SameSite = "lax";
                    break;
                case "2":
                    this.SameSite = "strict";
                    break;

                default:
                    this.SameSite = "unspecified";
                    break;
            }
        }
        public string ToNetscape()
        {
            // var expiryStr = ExpirationDate != null ? (ExpirationDate >= 0 ? TimeUtils.FromSecondsSinceEpoch((double)ExpirationDate) : DateTime.MinValue).ToString(CultureInfo.InvariantCulture).ToUpper() : null;

            var sameSiteStr = "Unspecified";
            if (SameSite?.Contains("lax") ?? false) sameSiteStr = "Lax";
            if (SameSite?.Contains("strict") ?? false) sameSiteStr = "Strict";

            return Domain + "\t" + (!string.IsNullOrWhiteSpace(Domain)).ToString(CultureInfo.InvariantCulture).ToUpper() + "\t" + Path + "\t" + (Secure.ToString(CultureInfo.InvariantCulture).ToUpper() ?? "FALSE") + "\t" +
                    /*expiryStr*/forDebug + "\t" + Name + "\t" + Value;// + "\t" + (HttpOnly.ToString(CultureInfo.InvariantCulture).ToUpper() ?? "FALSE") + "\t" +
                                                                       //(ExpirationDate == null || ExpirationDate < 0.01).ToString(CultureInfo.InvariantCulture).ToUpper() + "\t" + sameSiteStr + "\t" + "Medium";

        }
        public string ToJSON()
        {
            Type type = this.GetType();
            PropertyInfo[] properties = type.GetProperties();
            if (properties == null | properties.Length == 0)
                return "";
            List<string> jsonItems = new List<string>(); // Number of items in EditThisCookie
            for (int i = 0; i < properties.Length; i++)
            {
                PropertyInfo property = properties[i];
                object[] keyvalues = { property.Name[0].ToString().ToLower() + property.Name.Substring(1, property.Name.Length - 1), property.GetValue(this, null) };
                if (keyvalues[1] != null && keyvalues[1].ToString().Contains("\""))
                {
                    keyvalues[1] = keyvalues[1].ToString().Replace("\"", "\\\"");
                }
                string jsonString = "";
                if (keyvalues[0].ToString() == "expirationDate" && keyvalues[1].ToString() == "0")
                    continue;
                if (keyvalues[1] == null)
                    jsonString = String.Format("    \"{0}\": null", keyvalues[0]);
                else if (keyvalues[1].GetType() == typeof(String))
                {
                    jsonString = String.Format("    \"{0}\": \"{1}\"", keyvalues);
                }
                else if (keyvalues[1].GetType() == typeof(Boolean))
                {
                    keyvalues[1] = keyvalues[1].ToString().ToLower();
                    jsonString = String.Format("    \"{0}\": {1}", keyvalues);
                }
                else
                {
                    jsonString = String.Format("    \"{0}\": {1}", keyvalues);
                }
                if (jsonString != "")
                    jsonItems.Add(jsonString);
            }
            string results = "{\n" + String.Join(",\n", jsonItems.ToArray()) + "\n}";
            return results;
        }
    }
}
