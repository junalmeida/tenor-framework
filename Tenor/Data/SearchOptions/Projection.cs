/*
 * Licensed under the MIT License:
 * http://www.opensource.org/licenses/mit-license.php
 * 
 * Copyright (c) 2009 Marcos Almeida Jr, Rachel Carvalho and Vinicius Barbosa.
 *
 * See the file license.txt for copying permission.
 */
using System;

namespace Tenor.Data
{
    /// <summary>
    /// Represents a projection.
    /// </summary>
    public class Projection
    {

        public Projection(string propertyName)
            : this(propertyName, null)
        {
        }
        public Projection(string propertyName, string jointAlias)
        {
            if (string.IsNullOrEmpty(propertyName))
                throw new ArgumentNullException("propertyName");
            JoinAlias = jointAlias;
            PropertyName = propertyName;


            if (JoinAlias != null)
                if (JoinAlias.Contains("\"") || JoinAlias.Contains("\'") || JoinAlias.Contains("\r") || JoinAlias.Contains("\n") || JoinAlias.Contains(" "))
                {
                    throw (new ArgumentException("Invalid characters on the alias.", "jointAlias"));
                }
        }

        public string JoinAlias
        {
            get;
            private set;
        }

        public string PropertyName
        {
            get;
            private set;
        }



        public override bool Equals(object obj)
        {
            Projection y = obj as Projection;
            if (y == null)
                return false;
            else if (this.GetType() != y.GetType())
                return false;
            else
            {
                return (this.JoinAlias == y.JoinAlias && this.PropertyName == y.PropertyName);
            }
        }

        public override int GetHashCode()
        {
            int hash = 57;
            if (PropertyName != null)
                hash = 27 * hash * PropertyName.GetHashCode();
            if (JoinAlias != null)
                hash = 27 * hash * JoinAlias.GetHashCode();
            return hash;
        }


        public override string ToString()
        {
            if (JoinAlias != null)
                return string.Format("{0}.{1}", JoinAlias, PropertyName);
            else
                return PropertyName;
        }
    }


}