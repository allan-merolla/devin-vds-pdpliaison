using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography.X509Certificates;

namespace PdpLiaison
{
    internal class dnAttributeValue
    {
        internal string attribute;
        internal string value;
        internal char trailing;

        public dnAttributeValue()
        {
            this.attribute = "";
            this.value = "";
            this.trailing = ',';
        }

        public override string ToString()
        {
            string retVal;

            retVal = attribute + "=";

            if (hasEscapeCharacter(value)) {
                retVal += "\"" + value + "\"";
            }
            else {
                retVal += value;
            }

            return retVal;
        }

        private bool hasEscapeCharacter(string val)
        {
            //  todo. replace this with   ---->    val.IndexOfAny( { '"', "#", ... } ) > 0
            if (val.Contains('"')) {
                return true;
            }
            if (val.Contains('#')) {
                return true;
            }
            if (val.Contains('+')) {
                return true;
            }
            if (val.Contains(',')) {
                return true;
            }
            if (val.Contains(';')) {
                return true;
            }
            if (val.Contains('<')) {
                return true;
            }
            if (val.Contains('=')) {
                return true;
            }
            if (val.Contains('>')) {
                return true;
            }
            if (val.Contains('\\')) {
                return true;
            }

            return false;
        }
    }

    internal class DistinguishedName
    {
        private List<dnAttributeValue> elements;

        public DistinguishedName()
        {
            elements = new List<dnAttributeValue>();
        }

        /// <summary>
        /// Instantiate an object of DistinguishedName type.
        /// </summary>
        /// <param name="dn">RFC4514 conformant</param>
        public DistinguishedName(string dn)
        {
            populateAttributeValues(dn);
        }

        private void populateAttributeValues(string dn)
        {
            int length = 0;
            dnAttributeValue element;
            char[] scape = { '\\' };
            int comma;
            int plus;

            elements = new List<dnAttributeValue>();

            while (dn.Length > 0) {
                element = new dnAttributeValue();
                
                //Get the attribute
                length = dn.IndexOf('=');
                element.attribute = dn.Substring(0, length);
                dn = dn.Remove(0, length + 1);

                //Get the Value
                element.value = "";
                do {
                    comma = int.MaxValue;
                    plus = int.MaxValue;
                    if (dn.Contains(',')) {
                        comma = dn.IndexOf(',', 1);
                        if (comma < 1) {
                            comma = int.MaxValue;
                        }
                    }
                    if (dn.Contains('+')) {
                        plus = dn.IndexOf('+', 1);
                        if (plus < 1) {
                            plus = int.MaxValue;
                        }
                    }
                    length = Math.Min(comma, plus);
                    length = Math.Min(length, dn.Length);
                    element.value = element.value.TrimEnd(scape);
                    element.value += dn.Substring(0, length);
                    if (!element.value.EndsWith("\\")) {
                        length++;
                        if (plus < comma) {
                            element.trailing = '+';
                        }
                    }
                    length = Math.Min(dn.Length, length);
                    dn = dn.Remove(0, length);
                } while (element.value.EndsWith("\\"));

                element.value = element.value.Replace("\\\\", "!&#@");
                element.value = element.value.Replace("\\", "");
                element.value = element.value.Replace("!&#@", "\\");
                element.value = element.value.Replace("\"", "\\\"");

                //Add element
                elements.Add(element);
            }
        }

        internal X500DistinguishedName x500dn()
        {
            string convertedDnString = "";

            for (int i = 0; i < elements.Count; i++) {
                convertedDnString += elements[i].ToString();
                if (i < elements.Count - 1) {
                    convertedDnString += elements[i].trailing + " ";
                }
            }

            X500DistinguishedName dn = new X500DistinguishedName(convertedDnString);

            return dn;
        }
    }
}
