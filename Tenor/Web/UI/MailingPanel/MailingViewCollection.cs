using System.Diagnostics;
using System;
using System.Collections;
using Tenor.Data;
using System.Data;
using System.Collections.Generic;
using System.IO;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Security.Permissions;


namespace Tenor.Web.UI.WebControls.Core
{


    [DefaultProperty("Item"), AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public class MailingViewCollection : ViewCollection
    {


        /// <summary>
        /// </summary>
        /// <param name="owner"></param>
        public MailingViewCollection(Control owner)
            : base(owner)
        {
        }

        public override void Add(Control view)
        {
            if (view == null)
                throw new ArgumentNullException("view");

            if (Count == 0)
            {
                if (view.GetType() != typeof(FormView))
                {
                    throw (new ArgumentException("First view must be a FormView."));
                }
                base.Add(view);
            }
            else if (Count == 1)
            {
                if (view.GetType() != typeof(ResultView))
                {
                    throw (new ArgumentException("Second view must be a ResultView."));
                }
                base.Add(view);
            }
            else
            {
                throw (new ArgumentException("You can only add one FormView and one ResultView."));
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override void AddAt(int index, Control v)
        {
        }


        /// <summary></summary>
        /// <param name="index">The index of the view.</param>
        /// <returns></returns>
        public new View this[int index]
        {
            get
            {
                return ((View)(base[index]));
            }
        }

    }
}
