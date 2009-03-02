
using System.Collections.Generic;
using System.Configuration;
using System;

using Tenor.BLL;
using Tenor.Data;

namespace SampleApp.Business.Entities
{
/// <summary>
/// Represents the table Persons.
/// Some description on table.
/// </summary>
[Serializable(), Table("Persons", "dbo")]
public partial class Persons : BLLBase
{

#region Properties


/// <summary>
/// Represents the field PersonId.
/// 
/// </summary>
[Field(PrimaryKey=trueAutoKey=true)]
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
/// Represents the field Name.
/// 
/// </summary>
[Field()]
public string Name
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
/// Represents the field Email.
/// 
/// </summary>
[Field()]
public string Email
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

public const string PersonId = "PersonId";


public const string Name = "Name";


public const string Email = "Email";


}


#endregion
#region Foreign Keys


/// <summary>
/// Represents the relationship FK_Persons_Items_Persons.
/// </summary>
[ForeignKey(PersonId, PersonId)]

public BLLCollection<Persons_Items> Persons_sItemss
{
get
{
return (BLLCollection<Persons_Items>)GetPropertyValue();
}
}


#endregion
#region Constructors And Metadata


public Persons()
{ }


public Persons(bool lazyLoadingDisabled) :
base(lazyLoadingDisabled)
{ }


/// <summary>
/// Loads Persons from the database with these keys.
/// </summary><%
public Persons(int PersonId) :
base()
{
         this.PersonId = pPersonId;

Bind();
}


#endregion
#region Search


#endregion
}
}

