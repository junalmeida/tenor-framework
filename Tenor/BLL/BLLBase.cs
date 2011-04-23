/*
 * Licensed under the MIT License:
 * http://www.opensource.org/licenses/mit-license.php
 * 
 * Copyright (c) 2009 Marcos Almeida Jr, Rachel Carvalho and Vinicius Barbosa.
 *
 * See the file license.txt for copying permission.
 */


using System;
namespace Tenor.BLL
{

    ///// <seealso cref="zeus.htm"/> 
    /// <summary>
    /// This is the base entity class. All of your class must inherit directly or indirectly from this
    /// class.
    /// <para>Children classes gain functions to read and save data to a DataBase.</para>
    /// </summary>
    /// <remarks>
    /// You can use the MyGeneration Template on file EntityBased.zeus to create your classes.
    /// </remarks>
    [Obsolete("Please use now EntityBase class on Tenor.Data", true)]
    public abstract partial class BLLBase : object
    {
        public BLLBase()
        {
            throw new InvalidOperationException();
        }
    }
}