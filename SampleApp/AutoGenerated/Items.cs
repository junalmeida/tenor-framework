
using System.Collections.Generic;
using System.Configuration;
using System;

using Tenor.BLL;
using Tenor.Data;

namespace SampleApp.Business.Entities
{
/// <summary>
/// Represents the table Items.
/// This is an item.
/// </summary>
[Serializable(), Table("Items", "dbo")]
public partial class Items : BLLBase
{

#region Properties


/// <summary>
/// Represents the field ItemId.
/// 
/// </summary>
[Field(PrimaryKey=trueAutoKey=true)]
public int ItemId
{
get
{
return (returnType)GetPropertyValue();
}
set
{
				SetPropertyValue(value);
}
}


/// <summary>
/// Represents the field Description.
/// The description of this item.
/// </summary>
[Field()]
public string Description
{
get
{
return (returnType)GetPropertyValue();
}
set
{
				SetPropertyValue(value);
}
}


/// <summary>
/// Represents the field CategoryId.
/// Used to hold category id.
/// </summary>
[Field()]
public int CategoryId
{
get
{
return (returnType)GetPropertyValue();
}
set
{
				SetPropertyValue(value);
}
}


/// <summary>
/// Keeps a list of constants with property names.
/// </summary>
public partial class Properties : object {
private Properties() { }

public const string ItemId = "ItemId";


public const string Description = "Description";


public const string CategoryId = "CategoryId";


}


#endregion
#region Foreign Keys


/// <summary>
/// Represents the relationship FK_Persons_Items_Items.
/// </summary>
[ForeignKey(ItemId, ItemId)]

public BLLCollection<Persons_Items> Persons_sItemss
{
get
{
return (BLLCollection<Persons_Items>)GetPropertyValue();
}
}


/// <summary>
/// Represents the relationship FK_Items_Categories.
/// </summary>
[ForeignKey(CategoryId, CategoryId)]

public Categories Categories
{
get
{
return (Categories)GetPropertyValue();
}
set
{
SetPropertyValue(value);
}
}


#endregion
#region Constructors And Metadata


public Items()
{ }


public Items(bool lazyLoadingDisabled) :
base(lazyLoadingDisabled)
{ }


/// <summary>
/// Loads Items from the database with these keys.
/// </summary><%
public Items(int ItemId) :
base()
{
         this.ItemId = pItemId;

Bind();
}


#endregion
#region Search


#endregion
}
}

