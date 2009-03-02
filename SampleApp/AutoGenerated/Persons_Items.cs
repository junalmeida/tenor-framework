
using System.Collections.Generic;
using System.Configuration;
using System;

using Tenor.BLL;
using Tenor.Data;

namespace SampleApp.Business.Entities
{
/// <summary>
/// Represents the table Persons_Items.
/// Some descrition of this relation.
/// </summary>
[Serializable(), Table("Persons_Items", "dbo")]
public partial class Persons_Items : BLLBase
{

#region Properties


/// <summary>
/// Represents the field ItemId.
/// 
/// </summary>
[Field(PrimaryKey=true)]
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
/// Represents the field PersonId.
/// 
/// </summary>
[Field(PrimaryKey=true)]
public int PersonId
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


public const string PersonId = "PersonId";


}


#endregion
#region Foreign Keys


/// <summary>
/// Represents the relationship FK_Persons_Items_Items.
/// </summary>
[ForeignKey(ItemId, ItemId)]

public Items Items
{
get
{
return (Items)GetPropertyValue();
}
set
{
SetPropertyValue(value);
}
}


/// <summary>
/// Represents the relationship FK_Persons_Items_Persons.
/// </summary>
[ForeignKey(PersonId, PersonId)]

public Persons Persons
{
get
{
return (Persons)GetPropertyValue();
}
set
{
SetPropertyValue(value);
}
}


#endregion
#region Constructors And Metadata


public Persons_Items()
{ }


public Persons_Items(bool lazyLoadingDisabled) :
base(lazyLoadingDisabled)
{ }


/// <summary>
/// Loads Persons_Items from the database with these keys.
/// </summary><%
public Persons_Items(int ItemId, int PersonId) :
base()
{
         this.ItemId = pItemId;
         this.PersonId = pPersonId;

Bind();
}


#endregion
#region Search


#endregion
}
}

