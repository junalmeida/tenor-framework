/*
 * Licensed under the MIT License:
 * http://www.opensource.org/licenses/mit-license.php
 * 
 * Copyright (c) 2009 Marcos Almeida Jr, Rachel Carvalho and Vinicius Barbosa.
 *
 * See the file license.txt for copying permission.
 */
//TODO: Refactoring of bulk insert to be able to use the new dialects. 
//The commented code below was made on top of pure t-sql.


/*
using System.Diagnostics;
using System;
using System.Collections;
using Tenor.Data;
using System.Data;
using System.Collections.Generic;
using System.IO;
using System.Configuration;
 */
/*
  using DataRow = System.Data.DataRow;
  using DataSet = System.Data.DataSet;
 */
/*
using DbType = System.Data.DbType;
using Tenor.BLL;
using System.Data.Common;


namespace Tenor.Data
{

    /// <summary>
    /// Classe que implementa operações de inserção em lote.
    /// </summary>
    /// <remarks></remarks>
    public class BulkInsert : System.Collections.ObjectModel.Collection<BLL.EntityBase>
    {


        /// <summary>
        /// Retorna o máximo de parâmetros por transação.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks>Se você separar seu BulkInsert em partes, cada parte será uma transação independente.</remarks>
        public int MaxParameterCount
        {
            get
            {
                return 2100;
            }
        }


        //Public Const LastIdentity As String = "@BULK_SCOPE_IDENTITY"
        #region " Modelos "
        private const string BulkSql = "" + "SET XACT_ABORT ON" + "\r\n" + "{0}" + "\r\n" + "BEGIN TRANSACTION" + "\r\n" + "" + "\r\n" + "--Variáveis:" + "\r\n" + "{1}" + "\r\n" + "--Instruções:" + "\r\n" + "{2}" + "\r\n" + "" + "\r\n" + "" + "\r\n" + "IF @@ERROR <> 0 BEGIN" + "\r\n" + "   ROLLBACK TRANSACTION" + "\r\n" + "END ELSE BEGIN " + "\r\n" + "   COMMIT TRANSACTION" + "\r\n" + "END" + "\r\n" + "{3}" + "\r\n" + "RETURN" + "\r\n";

        private const string CreateSaveID = "CREATE TABLE {0} (" + "\r\n" + "   id int" + "\r\n" + ")";
        private const string SelectSaveID = "SELECT id FROM {0}" + "\r\n" + "DROP TABLE {0}" + "\r\n";


        private const string DeclareItem = "     DECLARE {0} int" + "\r\n";

        private const string BulkItem = "    {0}" + "\r\n" + "   SET {1} = SCOPE_IDENTITY()";
        //"   SET " & LastIdentity & " = SCOPE_IDENTITY()" & vbCrLf & _
        private const string BulkItemSaveId = "   INSERT INTO {0} VALUES ({1})" + "\r\n";

        #endregion

        #region " Contrutores e Variáveis "
        /// <summary>
        /// Inicia uma instancia do BulkInsert
        /// </summary>
        /// <remarks></remarks>
        public BulkInsert()
            : this(true)
        {
        }

        /// <summary>
        /// Inicia uma instancia do BulkInsert
        /// </summary>
        /// <param name="RetrieveIDs">Determina se deverá preencher as classes com novos IDs após a operação.</param>
        /// <remarks></remarks>
        public BulkInsert(bool RetrieveIDs)
            : this(true, null)
        {
        }

        /// <summary>
        /// Inicia uma instancia do BulkInsert
        /// </summary>
        /// <param name="RetrieveIDs">Determina se deverá preencher as classes com novos IDs após a operação.</param>
        /// <param name="Connection">Determina qual conexão usar no processo.</param>
        /// <remarks></remarks>
        public BulkInsert(bool RetrieveIDs, ConnectionStringSettings Connection)
        {
            this.Connection = Connection;

            _RetrieveIDs = RetrieveIDs;

            bulkTempTable = "#BULKINSERT" + Guid.NewGuid().ToString().Replace("{", "").Replace("}", "").Replace("-", "");
            parameters = new List<TenorParameter>(MaxParameterCount);
            bulkItems = new System.Text.StringBuilder();
            variaveis = new System.Text.StringBuilder();

            related = new List<List<Type>>();
        }

        private ConnectionStringSettings Connection;
        private string bulkTempTable;
        private List<TenorParameter> parameters;
        private System.Text.StringBuilder bulkItems;
        private System.Text.StringBuilder variaveis;
        private List<List<Type>> related;

        #endregion



        /// <summary>
        /// Adiciona um item na lista de inserts.
        ///
        /// </summary>
        /// <param name="Item">Item que deseja adicionar</param>
        /// <param name="RelateWith">Tipo de item que deseja relacionar. A relação será feita com o ultimo ID recuperado deste item.</param>
        /// <remarks></remarks>
        public void Add(BLL.EntityBase Item, Type RelateWith)
        {
            Add(Item, new Type[] { RelateWith });
        }

        /// <summary>
        /// Adiciona um item na lista de inserts.
        ///
        /// </summary>
        /// <param name="Item">Item que deseja adicionar</param>
        /// <param name="RelateWith">Tipos que deverá relacionar.</param>
        /// <remarks></remarks>
        public void Add(EntityBase Item, params Type[] RelateWith)
        {
            if (RelateWith == null)
            {
                throw (new ArgumentNullException("RelateWith"));
            }
            else if (RelateWith.Length == 0)
            {
                throw (new ArgumentOutOfRangeException("RelateWith"));
            }
            else
            {
                if (Array.IndexOf<Type>(RelateWith, null) > -1)
                {
                    throw (new ArgumentNullException("RelateWith"));
                }
            }
            base.Add(Item);
            related[this.Count - 1] = new List<Type>();
            related[this.Count - 1].AddRange(RelateWith);

            PrepareItem();
        }

        protected override void InsertItem(int index, BLL.EntityBase item)
        {
            if (item == null)
            {
                throw (new ArgumentNullException("RelateWith"));
            }
            base.InsertItem(index, item);
            related.Insert(index, new List<Type>());
        }

        protected override void RemoveItem(int index)
        {
            throw (new NotSupportedException());
            //MyBase.RemoveItem(index)
            //related.RemoveAt(index)
        }

        public new void Add(EntityBase item)
        {
            base.Add(item);

            PrepareItem();
        }








        /// <summary>
        /// Executa uma operação de inclusão em lotes
        /// </summary>
        /// <remarks></remarks>
        public virtual void Execute()
        {


            string createSqlIds = "";
            string selectSqlIds = "";
            if (this.RetrieveIDs)
            {
                createSqlIds = string.Format(CreateSaveID, bulkTempTable, null);
                selectSqlIds = string.Format(SelectSaveID, bulkTempTable, null);
            }

            string sql = string.Format(BulkSql, createSqlIds, variaveis.ToString(), bulkItems.ToString(), selectSqlIds);

            Tenor.Data.DataTable rs = null;
            Tenor.Diagnostics.Debug.DebugSQL(this.GetType().Name, sql, parameters.ToArray(), Connection);
            try
            {
                //Dim rs As Data.DataTable = Helper.ConsultarBanco(Connection, sql, params)
                rs = new Tenor.Data.DataTable(sql, parameters.ToArray(), Connection);
                Diagnostics.Debug.PrintCurrentTime("Bulk Insert - Sql Start");
                rs.Bind();
                Diagnostics.Debug.PrintCurrentTime("Bulk Insert - Sql Finish (" + this.Count.ToString() + " items, " + ParameterCount.ToString() + " parameters)");
            }
            catch (Exception ex)
            {
                throw (new InvalidOperationException("Cannot complete Bulk operation. Check inner exception for details." + "\r\n", ex));
            }

            if (RetrieveIDs && (rs != null))
            {

                for (int i = 0; i <= rs.Rows.Count - 1; i++)
                {
                    FieldInfo[] fields = BLL.EntityBase.GetFields(this[i].GetType(), true);
                    foreach (FieldInfo f in fields)
                    {
                        if (f.AutoNumber)
                        {
                            f.SetPropertyValue(this[i], true, rs[i][0]);
                        }
                    }
                }
            }
        }



        /// <summary>
        /// Retorna o número de parametros atuais do BulkInsert.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public int ParameterCount
        {
            get
            {
                return this.parameters.Count;
            }
        }



        private void PrepareItem()
        {
            int i = this.Count - 1;
            BLL.EntityBase instance = this[i];

            string paramPrefix = "bulk" + (i + 1).ToString() + "_";


            System.Collections.Specialized.NameValueCollection specialValues = new System.Collections.Specialized.NameValueCollection();

            //procurar por foreignkeys
            if (i > 0 && related[i].Count > 0)
            {
                ForeignKeyInfo[] fks = BLL.EntityBase.GetForeignKeys(instance.GetType());
                //nenhuma foreign key disponível
                if (fks.Length == 0)
                {
                    throw (new InvalidOperationException("Cannot create relations \'" + instance.ToString() + "\' item. No foreign keys defined."));
                }
                else
                {

                    bool found = false;
                    //para cada chave extrangeira definida
                    foreach (ForeignKeyInfo fk in fks)
                    {
                        //para cada campo da outra ponta
                        foreach (FieldInfo field in fk.ForeignFields)
                        {
                            //se o campo for do tipo de um dos itens relacionados, gerar uma chave identity
                            int pos = related[i].IndexOf(field.RelatedProperty.DeclaringType);
                            if (pos > -1 && field.AutoNumber)
                            {
                                //procura o parameter correspondente
                                specialValues.Add(BLL.EntityBase.GetParamName(paramPrefix, field), "@bulk" + related[i][pos].Name.ToLower());
                                found = true;
                                break;
                            }
                        }
                    }
                    if (!found)
                    {
                        string res = instance.GetType().Name;
                        try
                        {
                            res = instance.ToString();
                        }
                        catch (Exception)
                        {
                        }
                        throw (new InvalidOperationException("Cannot create relations for \'" + res + "\' item."));
                    }
                }

            }
            string variavel = "@bulk" + instance.GetType().Name.ToLower();
            if (!variaveis.ToString().Contains(variavel))
            {
                variaveis.Append(string.Format(DeclareItem, variavel, null));
            }


            TenorParameter[] tenorParameters = null;

            FieldInfo autoKeyField = null; //Not used at this time.
            Tenor.Data.Dialects.IDialect dialect = null;
            bulkItems.AppendLine(string.Format(BulkItem, instance.GetSaveSql(false, Connection, specialValues, out autoKeyField, out tenorParameters, out dialect), variavel));
            if (this.RetrieveIDs)
            {
                bulkItems.AppendLine(string.Format(BulkItemSaveId, bulkTempTable, variavel));
            }

        }





        private bool _RetrieveIDs;
        /// <summary>
        /// Determina se deverá ser retornado os ids após a execução.
        /// O valor padrão é 'True'.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool RetrieveIDs
        {
            get
            {
                return _RetrieveIDs;
            }
        }


    }

}

*/