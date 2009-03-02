using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using SampleApp.Business.Entities;

/// <summary>
/// Summary description for Business
/// </summary>
public class Business
{
    public Business()
    {
    }

    public Person LoadPerson(int personId)
    {
        try
        {
            return new Person(personId);
        }
        catch (Exception ex)
        {
            throw new ApplicationException("Cannot load this person.", ex);
        }
    }

    public void Save(Person p)
    {
        Validate(p);
        try
        {
            p.Save(p.PersonId > 0);
        }
        catch (Exception ex)
        {
            throw new ApplicationException("Cannot save this person.", ex);
        }
    }

    private void Validate(Person p)
    {
        if (string.IsNullOrEmpty(p.Name))
            throw new ApplicationException("You must type a name.");
        if (!string.IsNullOrEmpty(p.Email) && !p.Email.Contains("@"))
            throw new ApplicationException("You must type a valid email address.");
    }
}
