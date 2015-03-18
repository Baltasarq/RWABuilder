using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace RWABuilder.Core.MenuComponents {
	public partial class Function {
		/// <summary>
		/// Represents arguments in functions.
		/// </summary>
		public class Argument: BaseArgument {
			public const string ArgumentTagName = "Argument";
			public const string TagViewer = "Viewer";
			public const string TagData = "Data";
			public const string TagDesc = "Description";
			public const string TagText = "Text";
			public const string TagValue = "Value";

			public const string EtqRequired = "Required";
			public const string EtqType = "Type";
			public const string EtqLang = "Language";
			public const string EtqReadOnly = "ReadOnly";
			public const string EtqValue = "Value";
			public const string EtqDepends = "DependsFrom";
			public const string EtqAllowMultiSelect = "AllowMultiSelect";

			public const string ValueSetSeparator = ",";

            public enum ViewerType {
                Plain, DataColumns, DataValues, Map, TaxTree,
                SimpleValueSet, MultiValueSet,
                SimpleColorPicker, MultiColorPicker
            };

			/// <summary>
			/// Initializes a new instance of the <see cref="RWABuilder.Core.MenuComponents.Function+FunctionArgument"/> class.
			/// </summary>
			/// <param name="name">Name.</param>
			public Argument(string name, Function owner)
				: base( name, owner )
			{
                this.depends = "";
                this.value = "";
				this.desc = "";
				this.valueSet = new List<string>();
			}

			/// <summary>
			/// Gets or sets a value indicating whether this argument is required.
			/// </summary>
			/// <value><c>true</c> if this instance is required; otherwise, <c>false</c>.</value>
			public bool IsRequired {
				get {
					return this.required;
				}
				set {
					this.required = value;
					this.SetNeedsSave();
				}
			}

			/// <summary>
			/// Gets or sets a value indicating whether this argument is read only.
			/// </summary>
			/// <value><c>true</c> if this instance is read only; otherwise, <c>false</c>.</value>
			public bool IsReadOnly {
				get {
					return this.readOnly;
				}
				set {
					this.readOnly = value;
					this.SetNeedsSave();
				}
			}

			/// <summary>
			/// Gets or sets the viewer.
			/// </summary>
			/// <value>The viewer, as a <see cref="ViewerType"/>.</value>
			public ViewerType Viewer {
				get {
					return this.viewer;
				}
				set {
					this.viewer = value;
					this.SetNeedsSave();
				}
			}

			/// <summary>
			/// Gets or sets dependency of this argument.
			/// </summary>
			/// <value>The dependency info, as string.</value>
			public string DependsFrom {
				get {
					return this.depends;
				}
				set {
					this.depends = value.Trim();
					this.SetNeedsSave();
				}
			}

			/// <summary>
			/// Gets or sets the value of this argument.
			/// </summary>
			/// <value>The value, as a string.</value>
			public string Value {
				get {
					return this.value;
				}
				set {
					this.value = value.Trim();
					this.SetNeedsSave();
				}
			}

			/// <summary>
			/// Gets or sets a value indicating whether this <see cref="RWABuilder.Core.MenuComponents.Function+Argument"/> allow multiselect.
			/// </summary>
			/// <value><c>true</c> if allow multiselect; otherwise, <c>false</c>.</value>
			public bool AllowMultiselect {
				get {
					return this.multiSelect;
				}
				set {
					this.multiSelect = value;
					this.SetNeedsSave();
				}
			}

			/// <summary>
			/// Gets or sets the description.
			/// </summary>
			/// <value>The description.</value>
			public string Description {
				get {
					return this.desc;
				}
				set {
					this.desc = value;
				}
			}

			/// <summary>
			/// Gets or sets the value set for this argument.
			/// The value set is only relevant when the viewer is a Simple/MultiValueSet.
			/// </summary>
			/// <value>The value set.</value>
			public string[] ValueSet {
				get {
					return this.valueSet.ToArray();
				}
				set {
					this.valueSet.Clear();
					this.valueSet.AddRange( value );
				}
			}

			/// <summary>
			/// Gets the value set as string.
			/// </summary>
			/// <returns>The value set as a list in a string.</returns>
			public string GetValueSetAsString()
			{
				StringBuilder toret = new StringBuilder();
				string[] valueSet = this.ValueSet;

				for(int i = 0; i < valueSet.Length; ++i) {
					toret.Append( valueSet[ i ] );

					if ( i < ( valueSet.Length - 1 ) ) {
						toret.Append( ValueSetSeparator );
					}
				}

				return toret.ToString();
			}

			/// <summary>
			/// Gets a value indicating whether this <see cref="RWABuilder.Core.MenuComponents.Function+Argument"/> needs value set.
			/// </summary>
			/// <value><c>true</c> if needs value set; otherwise, <c>false</c>.</value>
			public bool NeedsValueSet {
				get {
					return ( this.viewer == ViewerType.SimpleValueSet
						  || this.viewer == ViewerType.MultiValueSet );
				}
			}

			/// <summary>
			/// Copies this Argument.
			/// </summary>
            /// <param name="newOwner">
			/// The <see cref="Function"/> which will be the owner of the copy.
			/// </param>
			/// <returns>
			/// A new <see cref="FunctionArgument"/>, which is an exact copy of this one.
			/// </returns>
			public override MenuComponent Copy(MenuComponent newOwner)
			{
                var functionOwner = newOwner as Function;

                if ( functionOwner == null ) {
					throw new ArgumentException( "need a function for the owner of copied Argument" );
				}

                var toret = new Argument( this.Name, functionOwner ) {
					IsRequired = this.IsRequired,
					AllowMultiselect = this.AllowMultiselect,
					Viewer = this.Viewer,
					DependsFrom = this.DependsFrom,
					Value = this.Value,
					ValueSet = this.ValueSet,
					Description = this.Description
				};

                return toret;
			}

			public override string ToString()
			{
				return string.Format( "[Argument: IsRequired={0}, Viewer={1}, "
				                     + "DependsFrom={2}, Tag={3}, AllowMultiselect={4}]",
				                     IsRequired, Viewer, DependsFrom, Value, AllowMultiselect );
			}

			/// <summary>
			/// Converts this argument to XML.
			/// </summary>
			/// <param name="doc">The document, as a XmlTextWriter.</param>
			public override void ToXml(XmlTextWriter doc)
			{
                Trace.WriteLine( "Function.ToXml(): " + this.ToString() );
                Trace.Indent();

				doc.WriteStartElement( ArgumentTagName );

				// Name = "arg1"
				doc.WriteStartAttribute( EtqName );
				doc.WriteString( this.Name );
				doc.WriteEndAttribute();

				// DependsFrom = "?"
				if ( !string.IsNullOrWhiteSpace( this.DependsFrom ) ) {
					doc.WriteStartAttribute( EtqDepends );
					doc.WriteString( this.DependsFrom );
					doc.WriteEndAttribute();
				}

				// Value = "9"
				if ( !string.IsNullOrWhiteSpace( this.Value ) ) {
					doc.WriteStartAttribute( EtqValue );
					doc.WriteString( this.Value );
					doc.WriteEndAttribute();
				}

				// IsRequired = "TRUE"
				if ( this.IsRequired ) {
					doc.WriteStartAttribute( EtqRequired );
					doc.WriteString( true.ToString().ToUpper() );
					doc.WriteEndAttribute();
				}

				// IsReadOnly = "TRUE"
				if ( this.IsReadOnly ) {
					doc.WriteStartAttribute( EtqReadOnly );
					doc.WriteString( true.ToString().ToUpper() );
					doc.WriteEndAttribute();
				}

				// AllowMultiSelect = "TRUE"
				if ( this.AllowMultiselect ) {
					doc.WriteStartAttribute( EtqAllowMultiSelect );
					doc.WriteString( true.ToString().ToUpper() );
					doc.WriteEndAttribute();
				}

				// <Viewer Type="Map"...
				doc.WriteStartElement( TagViewer );
				doc.WriteStartAttribute( EtqType );
				doc.WriteString( this.Viewer.ToString() );
				doc.WriteEndAttribute();

				if ( this.NeedsValueSet
				  && this.ValueSet.Length > 0 )
				{

					doc.WriteStartElement( TagData );
					doc.WriteString( this.GetValueSetAsString() );
					doc.WriteEndElement();
				}

				doc.WriteEndElement();

				// <Description...
				if ( !string.IsNullOrWhiteSpace( this.Description ) ) {
					doc.WriteStartElement( TagDesc );
					doc.WriteStartElement( TagText );
					doc.WriteStartAttribute( EtqLang );
					doc.WriteString( "ES" );
					doc.WriteEndAttribute();
					doc.WriteString( this.Description );
					doc.WriteEndElement();
					doc.WriteEndElement();
				}

				// <Value>...</Value>
				if ( !string.IsNullOrWhiteSpace( this.Value ) ) {
					doc.WriteStartElement( TagValue );
					doc.WriteString( this.Value );
					doc.WriteEndElement();
				}

				doc.WriteEndElement();
                Trace.Unindent();
				return;
			}

			public static Argument FromXml(XmlNode node, Function fn)
			{
                Trace.WriteLine( "Argument.FromXml: " + node.AsString() );

				string name = node.GetAttribute( EtqName ).InnerText;
				Argument toret = new Argument( name, fn );

				foreach(XmlAttribute attr in node.Attributes) {
					// Name = "arg"
					if ( attr.Name.Equals( EtqName, StringComparison.OrdinalIgnoreCase ) ) {
						toret.Name = attr.InnerText.Trim();
					}
					else
					// DependsFrom = "?"
					if ( attr.Name.Equals( EtqDepends, StringComparison.OrdinalIgnoreCase ) ) {
						toret.DependsFrom = attr.InnerText.Trim();
					}
					else
					// Tag = "?"
					if ( attr.Name.Equals( EtqValue, StringComparison.OrdinalIgnoreCase ) ) {
						toret.Value = attr.InnerText.Trim();
					}
					else
					// AllowMultiSelect = "TRUE"
					if ( attr.Name.Equals( EtqAllowMultiSelect, StringComparison.OrdinalIgnoreCase ) ) {
						toret.AllowMultiselect = attr.GetValueAsBool();
					}
					else
					// Required = "TRUE"
					if ( attr.Name.Equals( EtqRequired, StringComparison.OrdinalIgnoreCase ) ) {
						toret.IsRequired = attr.GetValueAsBool();
					}
					else
					// ReadOnly = "TRUE"
					if ( attr.Name.Equals( EtqReadOnly, StringComparison.OrdinalIgnoreCase ) ) {
						toret.IsReadOnly = attr.GetValueAsBool();
					}
				}

				// Explore subnodes...
				foreach(XmlNode subNode in node.ChildNodes) {
					// <Viewer Type="Map"...
					if ( subNode.Name.Equals( TagViewer, StringComparison.OrdinalIgnoreCase ) ) {
						ViewerType viewer;
						string viewerId = subNode.GetAttribute( EtqType ).InnerText;

						// Viewer Type
						if ( Enum.TryParse<ViewerType>( viewerId, true, out viewer ) ) {
							toret.Viewer = viewer;
						} else {
							throw new XmlException( "unknown viewer type: " + viewerId
								+ " at argument " + toret.Name );
						}

						// ValueSet
						foreach ( XmlNode subsubNode in subNode.ChildNodes ) {
							if ( subsubNode.Name.Equals( TagData, StringComparison.OrdinalIgnoreCase ) ) {
								string valueSet = subsubNode.InnerText;

								toret.ValueSet = valueSet.Split( ValueSetSeparator[ 0 ] );
							}
						}
					} else
					// <Description...
					if ( subNode.Name.Equals( TagDesc, StringComparison.OrdinalIgnoreCase ) ) {
						foreach ( XmlNode subsubNode in subNode.ChildNodes ) {
							if ( subsubNode.Name.Equals( TagText, StringComparison.OrdinalIgnoreCase ) ) {
								toret.Description = subsubNode.InnerText;
							}
						}
					} else
					// <Value>...</Value>
					if ( subNode.Name.Equals( TagValue, StringComparison.OrdinalIgnoreCase ) ) {
						toret.Value = subNode.InnerText;
					}
				}

				fn.RegularArgumentList.Add( toret );
				return toret;
			}

			private List<string> valueSet;
			private bool required;
			private bool readOnly;
			private bool multiSelect;
			private ViewerType viewer;
			private string depends;
			private string value;
			private string desc;
		}
	}
}

