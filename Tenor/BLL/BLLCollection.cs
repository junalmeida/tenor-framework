/*
 * Licensed under the MIT License:
 * http://www.opensource.org/licenses/mit-license.php
 * 
 * Copyright (c) 2009 Marcos Almeida Jr, Rachel Carvalho and Vinicius Barbosa.
 *
 * See the file license.txt for copying permission.
 */
using System;

namespace Tenor
{
    namespace BLL
    {
        /// <summary>
        /// Represents a collection of entities.
        /// </summary>
        /// <typeparam name="T">A Type that inherits directly or indirectly from EntityBase.</typeparam>
        [Serializable(), Obsolete("Please use IList or EntityList on Tenor.Data", true)]
        public class BLLCollection<T> where T : BLLBase
        {

        }


    }

}
