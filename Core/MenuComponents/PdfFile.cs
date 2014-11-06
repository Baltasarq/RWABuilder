﻿using System.Xml;

namespace RAppMenu.Core.MenuComponents {
    public class PdfFile: MenuComponent {
        public const string TagName = "PDF";
        public const string EtqName = "Name";

        public PdfFile(string fileName, Menu parent)
            : base( fileName, parent )
        {
        }

        /// <summary>
        /// Gets the file path to the PDF file.
        /// </summary>
        /// <value>The file path, as a string.</value>
        /// <seealso cref="RAppMenu.Core.MenuComponent.Name"/>
        public string FileName {
            get {
                return this.Name;
            }
        }

        public override void ToXml(XmlTextWriter doc)
        {
            doc.WriteStartElement( TagName );
            doc.WriteStartAttribute( EtqName );
            doc.WriteString( this.FileName );
            doc.WriteEndAttribute();
            doc.WriteEndElement();
        }
    }
}

