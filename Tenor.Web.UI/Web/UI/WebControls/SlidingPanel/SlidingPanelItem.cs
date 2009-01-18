using System.Diagnostics;
using System.Data;
using System.Collections;
using Microsoft.VisualBasic;
using System.Collections.Generic;
using System;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Security.Permissions;
using System.Reflection;


namespace Tenor
{
	namespace Web
	{
		namespace UI
		{
			namespace WebControls
			{
				
				
				[System.ComponentModel.ToolboxItemAttribute(false)]public class SlidingPanelItem : WebControl, IDataItemContainer, INamingContainer
				{
					
					
					
					internal void SetStyle(Style newStyle)
					{
						this.ApplyStyle(newStyle);
						if (Height.IsEmpty)
						{
							Height = Unit.Parse("100%");
						}
						Style["float"] = "left";
						
						
					}
					
					protected override System.Web.UI.HtmlTextWriterTag TagKey
					{
						get
						{
							return HtmlTextWriterTag.Div;
							
						}
					}
					
					
					protected override string TagName
					{
						get
						{
							return "div";
						}
					}
					
					
					internal SlidingPanelItem(int itemIndex, System.Web.UI.WebControls.ListItemType itemType)
					{
						_index = itemIndex;
						_ItemType = itemType;
					}
					
					private object _dataItem;
					public object DataItem
					{
						get
						{
							return _dataItem;
						}
					}
					
					private int _index = 0;
					public int DataItemIndex
					{
						get
						{
							return _index;
						}
					}
					
					public int DisplayIndex
					{
						get
						{
							return _index + 1;
						}
					}
					
					private ListItemType _ItemType;
					public ListItemType ItemType
					{
						get
						{
							return _ItemType;
						}
					}
					
					internal void SetDataItem(object item)
					{
						_dataItem = item;
					}
					
					
					protected override bool OnBubbleEvent(object source, EventArgs e)
					{
						if (e is CommandEventArgs)
						{
							SlidingPanelCommandEventArgs args = new SlidingPanelCommandEventArgs(this, source, ((CommandEventArgs) e));
							base.RaiseBubbleEvent(this, args);
							return true;
						}
						return false;
					}
					
					
				}
				
				
				
				[AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
                public sealed class SlidingPanelItemCollection : ICollection, IEnumerable
				{
					
					// Methods
					public SlidingPanelItemCollection(ArrayList items)
					{
						this.items = items;
					}
					
					public void CopyTo(Array array, int index)
					{
						IEnumerator enumerator = this.GetEnumerator();
						while (enumerator.MoveNext())
						{
							array.SetValue(enumerator.Current, index);
							index++;
						}
					}
					
					public IEnumerator GetEnumerator()
					{
						return this.items.GetEnumerator();
					}
					
					
					// Properties
					public int Count
					{
						get
						{
							return this.items.Count;
						}
					}
					
					public bool IsReadOnly
					{
						get
						{
							return false;
						}
					}
					
					public bool IsSynchronized
					{
						get
						{
							return false;
						}
					}
					
					public SlidingPanelItem this[int index]
					{
						get
						{
							return ((SlidingPanelItem) (this.items[index]));
						}
					}
					
					public object SyncRoot
					{
						get
						{
							return this;
						}
					}
					
					
					// Fields
					private ArrayList items;
				}
				
				
				
				
			}
		}
	}
	
}
