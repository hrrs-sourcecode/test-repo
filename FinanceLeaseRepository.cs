using DTO;
using DTO.Shared;
using Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Repositories
{
    public class FinanceLeaseRepository : IFinanceLeaseRepository
    {
        private string _connectionString = "";

        public FinanceLeaseRepository()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["PSAKSQLConnection"].ConnectionString;
        }

        public List<FinanceLease> GetAllFinanceLease(string currentUser)
        {
            List<FinanceLease> financeLeaseList = new List<FinanceLease>();

            using (SqlConnection sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                string query = "select *, 0 rk from FinanceLease where CategoryLeaseID = @pCategoryLeaseID and CreatedBy = @pCreatedBy" +
                       " union select *, 1 rk from FinanceLease where CategoryLeaseID != @pCategoryLeaseID order by rk asc, CreatedDate desc";

                using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
                {
                    sqlCommand.CommandType = System.Data.CommandType.Text;
                    sqlCommand.Parameters.AddWithValue("@pCreatedBy", currentUser);
                    sqlCommand.Parameters.AddWithValue("@pCategoryLeaseID", (int)CategoryLease.Draft);

                    using (SqlDataReader dataReader = sqlCommand.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            financeLeaseList.Add(new FinanceLease
                            {
                                ID = (dataReader["ID"].ToString()),
                                CategoryLeaseID = dataReader["CategoryLeaseID"] == DBNull.Value ? null : (int?)dataReader["CategoryLeaseID"],
                                RefNo = dataReader["RefNo"] == DBNull.Value ? null : dataReader["RefNo"].ToString(),
                                PrevRefNo = dataReader["PrevRefNo"] == DBNull.Value ? null : dataReader["PrevRefNo"].ToString(),
                                StatusLeaseID = dataReader["StatusLeaseID"] == DBNull.Value ? null : (int?)dataReader["StatusLeaseID"],
                                TypeLeaseCode = dataReader["TypeLeaseCode"] == DBNull.Value ? null : dataReader["TypeLeaseCode"].ToString(),

                                Lessor = dataReader["Lessor"] == DBNull.Value ? null : dataReader["Lessor"].ToString(),
                                Location = dataReader["Location"] == DBNull.Value ? null : dataReader["Location"].ToString(),
                                CcBcLeaseCode = dataReader["CcBcLeaseCode"] == DBNull.Value ? null : dataReader["CcBcLeaseCode"].ToString(),
                                StartDate = dataReader["StartDate"] == DBNull.Value ? null : (DateTime?)dataReader["StartDate"],
                                EndDate = dataReader["EndDate"] == DBNull.Value ? null : (DateTime?)dataReader["EndDate"],

                                PaymentMethodLeaseID = dataReader["PaymentMethodLeaseID"] == DBNull.Value ? null : (int?)dataReader["PaymentMethodLeaseID"],
                                CurrencyLeaseCode = dataReader["CurrencyLeaseCode"] == DBNull.Value ? null : dataReader["CurrencyLeaseCode"].ToString(),
                                PaymentAmount = dataReader["PaymentAmount"] == DBNull.Value ? null : (double?)(decimal)dataReader["PaymentAmount"],
                                AccTypeID = dataReader["AccTypeID"] == DBNull.Value ? null : (int?)dataReader["AccTypeID"],
                                IsPaidFront = (bool)dataReader["IsPaidFront"],
                                IsNeedEstimation = (bool)dataReader["IsNeedEstimation"],
                                LeaseTerm = dataReader["LeaseTerm"] == DBNull.Value ? null : (int?)dataReader["LeaseTerm"],
                                PaymentTerm = dataReader["PaymentTerm"] == DBNull.Value ? null : (int?)dataReader["PaymentTerm"],
                                ReconditionCost = dataReader["ReconditionCost"] == DBNull.Value ? null : (double?)(decimal)dataReader["ReconditionCost"],
                                AdjReconditionCost = dataReader["AdjReconditionCost"] == DBNull.Value ? null : (double?)(decimal)dataReader["AdjReconditionCost"],
                                ReaReconditionCost = dataReader["ReaReconditionCost"] == DBNull.Value ? null : (double?)(decimal)dataReader["ReaReconditionCost"],
                                APLeasingMigration = dataReader["APLeasingMigration"] == DBNull.Value ? null : (double?)(decimal)dataReader["APLeasingMigration"],
                                PrepaidMigration = dataReader["PrepaidMigration"] == DBNull.Value ? null : (double?)(decimal)dataReader["PrepaidMigration"],

                                IsActive = (bool)dataReader["IsActive"],
                                CreatedBy = dataReader["CreatedBy"].ToString(),
                                CreatedDate = (DateTime)dataReader["CreatedDate"],
                                ModifiedBy = dataReader["ModifiedBy"].ToString(),
                                ModifiedDate = dataReader["ModifiedDate"] == DBNull.Value ? null : (DateTime?)dataReader["ModifiedDate"]
                            });
                        }
                    }
                }
                sqlConnection.Close();
            }
            return financeLeaseList;
        }

        public List<FinanceLease> GetFinanceLeaseByListCriteria(IList<SearchCriteria> listCriteria, int startRow = 1, int totalRow = 10)
        {
            List<FinanceLease> financeLeaseList = new List<FinanceLease>();

            using (SqlConnection sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();

                string query = "select * from FinanceLease where ";

                foreach (SearchCriteria item in listCriteria.Distinct())
                {
                    query += item.PropertyName + item.Relation + "@p" + item.PropertyName;
                    if (listCriteria.IndexOf(item) != listCriteria.Count() - 1)
                    {
                        query += " and ";
                    }
                };

                query += " order by CreatedDate desc";

                using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
                {
                    sqlCommand.CommandType = System.Data.CommandType.Text;
                    foreach (SearchCriteria item in listCriteria.Distinct())
                    {
                        sqlCommand.Parameters.AddWithValue("@p" + item.PropertyName, item.Value);
                    }
                    using (SqlDataReader dataReader = sqlCommand.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            financeLeaseList.Add(new FinanceLease
                            {
                                ID = (dataReader["ID"].ToString()),
                                CategoryLeaseID = dataReader["CategoryLeaseID"] == DBNull.Value ? null : (int?)dataReader["CategoryLeaseID"],
                                RefNo = dataReader["RefNo"] == DBNull.Value ? null : dataReader["RefNo"].ToString(),
                                PrevRefNo = dataReader["PrevRefNo"] == DBNull.Value ? null : dataReader["PrevRefNo"].ToString(),
                                StatusLeaseID = dataReader["StatusLeaseID"] == DBNull.Value ? null : (int?)dataReader["StatusLeaseID"],
                                TypeLeaseCode = dataReader["TypeLeaseCode"] == DBNull.Value ? null : dataReader["TypeLeaseCode"].ToString(),

                                Lessor = dataReader["Lessor"] == DBNull.Value ? null : dataReader["Lessor"].ToString(),
                                Location = dataReader["Location"] == DBNull.Value ? null : dataReader["Location"].ToString(),
                                CcBcLeaseCode = dataReader["CcBcLeaseCode"] == DBNull.Value ? null : dataReader["CcBcLeaseCode"].ToString(),
                                StartDate = dataReader["StartDate"] == DBNull.Value ? null : (DateTime?)dataReader["StartDate"],
                                EndDate = dataReader["EndDate"] == DBNull.Value ? null : (DateTime?)dataReader["EndDate"],

                                PaymentMethodLeaseID = dataReader["PaymentMethodLeaseID"] == DBNull.Value ? null : (int?)dataReader["PaymentMethodLeaseID"],
                                CurrencyLeaseCode = dataReader["CurrencyLeaseCode"] == DBNull.Value ? null : dataReader["CurrencyLeaseCode"].ToString(),
                                PaymentAmount = dataReader["PaymentAmount"] == DBNull.Value ? null : (double?)(decimal)dataReader["PaymentAmount"],
                                AccTypeID = dataReader["AccTypeID"] == DBNull.Value ? null : (int?)dataReader["AccTypeID"],
                                IsPaidFront = (bool)dataReader["IsPaidFront"],
                                IsNeedEstimation = (bool)dataReader["IsNeedEstimation"],
                                LeaseTerm = dataReader["LeaseTerm"] == DBNull.Value ? null : (int?)dataReader["LeaseTerm"],
                                PaymentTerm = dataReader["PaymentTerm"] == DBNull.Value ? null : (int?)dataReader["PaymentTerm"],
                                ReconditionCost = dataReader["ReconditionCost"] == DBNull.Value ? null : (double?)(decimal)dataReader["ReconditionCost"],
                                AdjReconditionCost = dataReader["AdjReconditionCost"] == DBNull.Value ? null : (double?)(decimal)dataReader["AdjReconditionCost"],
                                ReaReconditionCost = dataReader["ReaReconditionCost"] == DBNull.Value ? null : (double?)(decimal)dataReader["ReaReconditionCost"],
                                APLeasingMigration = dataReader["APLeasingMigration"] == DBNull.Value ? null : (double?)(decimal)dataReader["APLeasingMigration"],
                                PrepaidMigration = dataReader["PrepaidMigration"] == DBNull.Value ? null : (double?)(decimal)dataReader["PrepaidMigration"],

                                IsActive = (bool)dataReader["IsActive"],
                                CreatedBy = dataReader["CreatedBy"].ToString(),
                                CreatedDate = (DateTime)dataReader["CreatedDate"],
                                ModifiedBy = dataReader["ModifiedBy"].ToString(),
                                ModifiedDate = dataReader["ModifiedDate"] == DBNull.Value ? null : (DateTime?)dataReader["ModifiedDate"]
                            });
                        }
                    }
                }
                sqlConnection.Close();
            }
            return financeLeaseList;
        }

        public FinanceLease GetFinanceLeaseByID(string ID)
        {
            FinanceLease financeLease = new FinanceLease();
            using (SqlConnection sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                string query = "select * from FinanceLease where ID = @pID";
                using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
                {
                    sqlCommand.CommandType = System.Data.CommandType.Text;
                    sqlCommand.Parameters.AddWithValue("@pID", ID);

                    using (SqlDataReader dataReader = sqlCommand.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            financeLease.ID = (dataReader["ID"].ToString());
                            financeLease.CategoryLeaseID = dataReader["CategoryLeaseID"] == DBNull.Value ? null : (int?)dataReader["CategoryLeaseID"];
                            financeLease.RefNo = dataReader["RefNo"] == DBNull.Value ? null : dataReader["RefNo"].ToString();
                            financeLease.PrevRefNo = dataReader["PrevRefNo"] == DBNull.Value ? null : dataReader["PrevRefNo"].ToString();
                            financeLease.StatusLeaseID = dataReader["StatusLeaseID"] == DBNull.Value ? null : (int?)dataReader["StatusLeaseID"];
                            financeLease.TypeLeaseCode = dataReader["TypeLeaseCode"] == DBNull.Value ? null : dataReader["TypeLeaseCode"].ToString();

                            financeLease.Lessor = dataReader["Lessor"] == DBNull.Value ? null : dataReader["Lessor"].ToString();
                            financeLease.Location = dataReader["Location"] == DBNull.Value ? null : dataReader["Location"].ToString();
                            financeLease.CcBcLeaseCode = dataReader["CcBcLeaseCode"] == DBNull.Value ? null : dataReader["CcBcLeaseCode"].ToString();
                            financeLease.StartDate = dataReader["StartDate"] == DBNull.Value ? null : (DateTime?)dataReader["StartDate"];
                            financeLease.EndDate = dataReader["EndDate"] == DBNull.Value ? null : (DateTime?)dataReader["EndDate"];

                            financeLease.PaymentMethodLeaseID = dataReader["PaymentMethodLeaseID"] == DBNull.Value ? null : (int?)dataReader["PaymentMethodLeaseID"];
                            financeLease.CurrencyLeaseCode = dataReader["CurrencyLeaseCode"] == DBNull.Value ? null : dataReader["CurrencyLeaseCode"].ToString();
                            financeLease.PaymentAmount = dataReader["PaymentAmount"] == DBNull.Value ? null : (double?)(decimal)dataReader["PaymentAmount"];
                            financeLease.AccTypeID = dataReader["AccTypeID"] == DBNull.Value ? null : (int?)dataReader["AccTypeID"];
                            financeLease.IsPaidFront = (bool)dataReader["IsPaidFront"];
                            financeLease.IsNeedEstimation = (bool)dataReader["IsNeedEstimation"];
                            financeLease.LeaseTerm = dataReader["LeaseTerm"] == DBNull.Value ? null : (int?)dataReader["LeaseTerm"];
                            financeLease.PaymentTerm = dataReader["PaymentTerm"] == DBNull.Value ? null : (int?)dataReader["PaymentTerm"];
                            financeLease.ReconditionCost = dataReader["ReconditionCost"] == DBNull.Value ? null : (double?)(decimal)dataReader["ReconditionCost"];
                            financeLease.AdjReconditionCost = dataReader["AdjReconditionCost"] == DBNull.Value ? null : (double?)(decimal)dataReader["AdjReconditionCost"];
                            financeLease.ReaReconditionCost = dataReader["ReaReconditionCost"] == DBNull.Value ? null : (double?)(decimal)dataReader["ReaReconditionCost"];
                            financeLease.APLeasingMigration = dataReader["APLeasingMigration"] == DBNull.Value ? null : (double?)(decimal)dataReader["APLeasingMigration"];
                            financeLease.PrepaidMigration = dataReader["PrepaidMigration"] == DBNull.Value ? null : (double?)(decimal)dataReader["PrepaidMigration"];

                            financeLease.IsActive = (bool)dataReader["IsActive"];
                            financeLease.CreatedBy = dataReader["CreatedBy"].ToString();
                            financeLease.CreatedDate = (DateTime)dataReader["CreatedDate"];
                            financeLease.ModifiedBy = dataReader["ModifiedBy"].ToString();
                            financeLease.ModifiedDate = dataReader["ModifiedDate"] == DBNull.Value ? null : (DateTime?)dataReader["ModifiedDate"];
                        }
                    }
                }

                financeLease.PaymentScheduleList = GetPaymentScheduleByFinanceLeaseID(financeLease.ID, sqlConnection);

                //query = "select * from PaymentScheduleLease where RefNo = @pRefNo";
                //using (SqlCommand sqlCommand1 = new SqlCommand(query, sqlConnection))
                //{
                //    sqlCommand1.CommandType = System.Data.CommandType.Text;
                //    sqlCommand1.Parameters.AddWithValue("@pRefNo", financeLease.RefNo);

                //    using (SqlDataReader dataReader = sqlCommand1.ExecuteReader())
                //    {
                //        if (dataReader.HasRows)
                //        {
                //            financeLease.PaymentScheduleList = new List<PaymentSchedule>();
                //        }
                //        while (dataReader.Read())
                //        {
                //            financeLease.PaymentScheduleList.Add
                //                (new PaymentSchedule
                //                {
                //                    ID = dataReader["ID"].ToString(),
                //                    Term = (int)dataReader["Term"],
                //                    PaymentAmount = (double)dataReader["PaymentAmount"],
                //                    PaymentDate = (DateTime)dataReader["PaymentDate"]
                //                });
                //        }
                //    }
                //}


                query = "select * from ActionHistory where FinanceLeaseID = @pFinanceLeaseID";
                using (SqlCommand sqlCommand1 = new SqlCommand(query, sqlConnection))
                {
                    sqlCommand1.CommandType = System.Data.CommandType.Text;
                    sqlCommand1.Parameters.AddWithValue("@pFinanceLeaseID", financeLease.ID);

                    using (SqlDataReader dataReader = sqlCommand1.ExecuteReader())
                    {
                        if (dataReader.HasRows)
                        {
                            financeLease.ActionHistoryList = new List<ActionHistory>();
                        }
                        while (dataReader.Read())
                        {
                            financeLease.ActionHistoryList.Add
                                (new ActionHistory
                                {
                                    ID = (dataReader["ID"].ToString()),
                                    ContractActionID = (int)dataReader["ContractActionID"],
                                    NPK = dataReader["NPK"].ToString(),
                                    Remarks = dataReader["Remarks"].ToString(),

                                    CreatedDate = (DateTime)dataReader["CreatedDate"],
                                    CreatedBy = dataReader["CreatedBy"].ToString()
                                });
                        }
                    }
                }

                sqlConnection.Close();
            }

            return financeLease;
        }

        public ResultStatus CreateFinanceLease(FinanceLease financeLease)
        {
            ResultStatus result = new ResultStatus();

            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    using (SqlConnection sqlConnection = new SqlConnection(_connectionString))
                    {
                        sqlConnection.Open();
                        string query = "insert into FinanceLease" +
                            "(ID,CategoryLeaseID,RefNo,PrevRefNo, StatusLeaseID,TypeLeaseCode,Lessor,Location,CcBcLeaseCode," +
                              "StartDate,EndDate,PaymentMethodLeaseID,CurrencyLeaseCode,PaymentAmount, AccTypeID," +
                              "IsPaidFront,IsNeedEstimation,CreatedBy,CreatedDate,IsActive,LeaseTerm, PaymentTerm, ReconditionCost, AdjReconditionCost, ReaReconditionCost,APLeasingMigration,PrepaidMigration)" +
                            "values" +
                            "(@pID,@pCategoryListID,@pRefNo,@pPrevRefNo, @pStatusLeaseID,@pTypeLeaseCode,@pLessor,@pLocation,@pCcBcLeaseCode," +
                            "@pStartDate, @pEndDate, @pPaymentMethodLeaseID, @pCurrencyLeaseCode, @pPaymentAmount, @pAccTypeID," +
                            "@pIsPaidFront, @pIsNeedEstimation, @pCreatedBy, @pCreatedDate, @pIsActive, @pLeaseTerm, @pPaymentTerm, @pReconditionCost, @pAdjReconditionCost, @pReaReconditionCost, @pAPLeasingMigration, @pPrepaidMigration)";

                        string newID = Guid.NewGuid().ToString();
                        financeLease.ID = newID;

                        // For New and Extend 
                        if (financeLease.CategoryLeaseID == (int?)CategoryLease.New || 
                            financeLease.CategoryLeaseID == (int?)CategoryLease.Extend ||
                            financeLease.CategoryLeaseID == (int?)CategoryLease.Draft)
                        {
                            financeLease.RefNo = GetRefNo(financeLease, sqlConnection);
                        }

                        // For Draft no action history needed
                        if (financeLease.CategoryLeaseID != (int?)CategoryLease.Draft)
                        {                         
                            result.Messages.AddRange(CreateActionHistory(financeLease, sqlConnection, newID).Messages);                           
                        }                        

                        using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
                        {
                            sqlCommand.CommandType = System.Data.CommandType.Text;
                            sqlCommand.Parameters.AddWithValue("@pID", financeLease.ID);
                            sqlCommand.Parameters.AddWithValue("@pCategoryListID", financeLease.CategoryLeaseID ?? (object)DBNull.Value);
                            sqlCommand.Parameters.AddWithValue("@pRefNo", financeLease.RefNo ?? (object)DBNull.Value);
                            sqlCommand.Parameters.AddWithValue("@pPrevRefNo", financeLease.PrevRefNo ?? (object)DBNull.Value);
                            sqlCommand.Parameters.AddWithValue("@pStatusLeaseID", financeLease.StatusLeaseID ?? (object)DBNull.Value);
                            sqlCommand.Parameters.AddWithValue("@pTypeLeaseCode", financeLease.TypeLeaseCode ?? (object)DBNull.Value);
                            sqlCommand.Parameters.AddWithValue("@pLessor", financeLease.Lessor ?? (object)DBNull.Value);
                            sqlCommand.Parameters.AddWithValue("@pLocation", financeLease.Location ?? (object)DBNull.Value);
                            sqlCommand.Parameters.AddWithValue("@pCcBcLeaseCode", financeLease.CcBcLeaseCode ?? (object)DBNull.Value);
                            sqlCommand.Parameters.AddWithValue("@pStartDate", financeLease.StartDate ?? (object)DBNull.Value);
                            sqlCommand.Parameters.AddWithValue("@pEndDate", financeLease.EndDate ?? (object)DBNull.Value);
                            sqlCommand.Parameters.AddWithValue("@pPaymentMethodLeaseID", financeLease.PaymentMethodLeaseID ?? (object)DBNull.Value);
                            sqlCommand.Parameters.AddWithValue("@pCurrencyLeaseCode", financeLease.CurrencyLeaseCode ?? (object)DBNull.Value);
                            sqlCommand.Parameters.AddWithValue("@pPaymentAmount", financeLease.PaymentAmount ?? (object)DBNull.Value);
                            sqlCommand.Parameters.AddWithValue("@pAccTypeID", financeLease.AccTypeID ?? (object)DBNull.Value);
                            sqlCommand.Parameters.AddWithValue("@pIsPaidFront", financeLease.IsPaidFront);
                            sqlCommand.Parameters.AddWithValue("@pIsNeedEstimation", financeLease.IsNeedEstimation);
                            sqlCommand.Parameters.AddWithValue("@pLeaseTerm", financeLease.LeaseTerm ?? (object)DBNull.Value);
                            sqlCommand.Parameters.AddWithValue("@pPaymentTerm", financeLease.PaymentTerm ?? (object)DBNull.Value);
                            sqlCommand.Parameters.AddWithValue("@pReconditionCost", financeLease.ReconditionCost ?? (object)DBNull.Value);
                            sqlCommand.Parameters.AddWithValue("@pAdjReconditionCost", financeLease.AdjReconditionCost ?? (object)DBNull.Value);
                            sqlCommand.Parameters.AddWithValue("@pReaReconditionCost", financeLease.ReaReconditionCost ?? (object)DBNull.Value);
                            sqlCommand.Parameters.AddWithValue("@pAPLeasingMigration", financeLease.APLeasingMigration ?? (object)DBNull.Value);
                            sqlCommand.Parameters.AddWithValue("@pPrepaidMigration", financeLease.PrepaidMigration ?? (object)DBNull.Value);

                            //TODO : Change the CreatedBy and CreatedDate after finished 
                            sqlCommand.Parameters.AddWithValue("@pCreatedBy", financeLease.CreatedBy);
                            sqlCommand.Parameters.AddWithValue("@pCreatedDate", DateTime.Now);
                            sqlCommand.Parameters.AddWithValue("@pIsActive", 1);

                            sqlCommand.ExecuteReader();
                            result.Messages.Add(string.Format("Table {0} has been successfully inserted!", "Finance Lease"));
                            result.IsSuccess = true;

                        }

                        if (financeLease.PaymentScheduleList != null )
                        {
                            query = "insert into FinancePaymentSchedule" +
                                "(ID,Term,FinanceLeaseID,PaymentDate,PaymentAmount,CreatedBy,CreatedDate,IsActive)" + 
                                "values" +
                                "(@pID,@pTerm,@pFinanceLeaseID,@pPaymentDate,@pPaymentAmount,@pCreatedBy,@pCreatedDate,@pIsActive)";
                            foreach (PaymentSchedule item in financeLease.PaymentScheduleList)
                            {
                                using (SqlCommand sqlCommand1 = new SqlCommand(query, sqlConnection))
                                {
                                    sqlCommand1.CommandType = System.Data.CommandType.Text;
                                    sqlCommand1.Parameters.AddWithValue("@pID", Guid.NewGuid().ToString());
                                    sqlCommand1.Parameters.AddWithValue("@pTerm", item.Term);
                                    sqlCommand1.Parameters.AddWithValue("@pFinanceLeaseID", newID);
                                    sqlCommand1.Parameters.AddWithValue("@pPaymentDate", item.PaymentDate);
                                    sqlCommand1.Parameters.AddWithValue("@pPaymentAmount", item.PaymentAmount);

                                    //TODO : Change the CreatedBy and CreatedDate after finished 
                                    sqlCommand1.Parameters.AddWithValue("@pCreatedBy", financeLease.CreatedBy);
                                    sqlCommand1.Parameters.AddWithValue("@pCreatedDate", DateTime.Now);
                                    sqlCommand1.Parameters.AddWithValue("@pIsActive", 1);

                                    sqlCommand1.ExecuteReader();
                                    result.Messages.Add(string.Format("Table {0} has been successfully inserted!", "Finance Lease"));
                                    result.IsSuccess = true;
                                }
                            }
                        }
                        sqlConnection.Close();
                    }

                    scope.Complete();
                };


            }
            catch (Exception exception)
            {
                result.Messages.Add(exception.Message);
                result.IsSuccess = false;
            }
            return result;
        }

        public ResultStatus UpdateFinanceLease(FinanceLease financeLease)
        {
            ResultStatus result = new ResultStatus();
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    using (SqlConnection sqlConnection = new SqlConnection(_connectionString))
                    {
                        sqlConnection.Open();
                        string query = "update FinanceLease set " +
                            "RefNo = @pRefNo, CategoryLeaseID = @pCategoryLeaseID, StatusLeaseID = @pStatusLeaseID ,TypeLeaseCode = @pTypeLeaseCode, Lessor = @pLessor ,Location = @pLocation ," + 
                            "CcBcLeaseCode = @pCcBcLeaseCode, StartDate = @pStartDate, EndDate = @pEndDate, PaymentMethodLeaseID = @pPaymentMethodLeaseID, CurrencyLeaseCode = @pCurrencyLeaseCode, PaymentAmount = @pPaymentAmount, " +
                            "AccTypeID = @pAccTypeID, IsPaidFront = @pIsPaidFront, IsNeedEstimation = @pIsNeedEstimation,LeaseTerm = @pLeaseTerm, PaymentTerm = @pPaymentTerm, ReconditionCost = @pReconditionCost, "+
                            "AdjReconditionCost = @pAdjReconditionCost,ReaReconditionCost = @pReaReconditionCost, APLeasingMigration = @pAPLeasingMigration, PrepaidMigration = @pPrepaidMigration, ModifiedBy=@pModifiedBy, ModifiedDate=@pModifiedDate, IsActive=@pIsActive " +
                            "where ID=@pID";

                        if (financeLease.RefNo.StartsWith("DRFT"))
                        {
                            financeLease.RefNo = GetRefNo(financeLease, sqlConnection);
                            query = "update FinanceLease set " +
                            "RefNo = @pRefNo, CategoryLeaseID = @pCategoryLeaseID, StatusLeaseID = @pStatusLeaseID ,TypeLeaseCode = @pTypeLeaseCode, Lessor = @pLessor ,Location = @pLocation ," +
                            "CcBcLeaseCode = @pCcBcLeaseCode, StartDate = @pStartDate, EndDate = @pEndDate, PaymentMethodLeaseID = @pPaymentMethodLeaseID, CurrencyLeaseCode = @pCurrencyLeaseCode, PaymentAmount = @pPaymentAmount, " +
                            "AccTypeID = @pAccTypeID, IsPaidFront = @pIsPaidFront, IsNeedEstimation = @pIsNeedEstimation,LeaseTerm = @pLeaseTerm, PaymentTerm = @pPaymentTerm, ReconditionCost = @pReconditionCost," +
                            "AdjReconditionCost = @pAdjReconditionCost,ReaReconditionCost = @pReaReconditionCost, APLeasingMigration = @pAPLeasingMigration, PrepaidMigration = @pPrepaidMigration, CreatedBy=@pCreatedBy, CreatedDate=@pCreatedDate, IsActive=@pIsActive " +
                            "where ID=@pID";
                        };

                        if (financeLease.CategoryLeaseID != (int?)CategoryLease.Draft)
                        {
                            result.Messages.AddRange(CreateActionHistory(financeLease, sqlConnection).Messages);
                        }               

                        using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
                        {
                            sqlCommand.CommandType = System.Data.CommandType.Text;

                            sqlCommand.Parameters.AddWithValue("@pID", financeLease.ID.ToString());
                            sqlCommand.Parameters.AddWithValue("@pCategoryLeaseID", financeLease.CategoryLeaseID ?? (object)DBNull.Value);
                            sqlCommand.Parameters.AddWithValue("@pRefNo", financeLease.RefNo ?? (object)DBNull.Value);
                            sqlCommand.Parameters.AddWithValue("@pPrevRefNo", financeLease.PrevRefNo ?? (object)DBNull.Value);
                            sqlCommand.Parameters.AddWithValue("@pStatusLeaseID", financeLease.StatusLeaseID ?? (object)DBNull.Value);
                            sqlCommand.Parameters.AddWithValue("@pTypeLeaseCode", financeLease.TypeLeaseCode ?? (object)DBNull.Value);
                            sqlCommand.Parameters.AddWithValue("@pLessor", financeLease.Lessor ?? (object)DBNull.Value);
                            sqlCommand.Parameters.AddWithValue("@pLocation", financeLease.Location ?? (object)DBNull.Value);
                            sqlCommand.Parameters.AddWithValue("@pCcBcLeaseCode", financeLease.CcBcLeaseCode ?? (object)DBNull.Value);
                            sqlCommand.Parameters.AddWithValue("@pStartDate", financeLease.StartDate ?? (object)DBNull.Value);
                            sqlCommand.Parameters.AddWithValue("@pEndDate", financeLease.EndDate ?? (object)DBNull.Value);
                            sqlCommand.Parameters.AddWithValue("@pPaymentMethodLeaseID", financeLease.PaymentMethodLeaseID ?? (object)DBNull.Value);
                            sqlCommand.Parameters.AddWithValue("@pCurrencyLeaseCode", financeLease.CurrencyLeaseCode ?? (object)DBNull.Value);
                            sqlCommand.Parameters.AddWithValue("@pPaymentAmount", financeLease.PaymentAmount ?? (object)DBNull.Value);
                            sqlCommand.Parameters.AddWithValue("@pAccTypeID", financeLease.AccTypeID ?? (object)DBNull.Value);
                            sqlCommand.Parameters.AddWithValue("@pIsPaidFront", financeLease.IsPaidFront);
                            sqlCommand.Parameters.AddWithValue("@pIsNeedEstimation", financeLease.IsNeedEstimation);
                            sqlCommand.Parameters.AddWithValue("@pLeaseTerm", financeLease.LeaseTerm ?? (object)DBNull.Value);
                            sqlCommand.Parameters.AddWithValue("@pPaymentTerm", financeLease.PaymentTerm ?? (object)DBNull.Value);
                            sqlCommand.Parameters.AddWithValue("@pReconditionCost", financeLease.ReconditionCost ?? (object)DBNull.Value);
                            sqlCommand.Parameters.AddWithValue("@pAdjReconditionCost", financeLease.AdjReconditionCost ?? (object)DBNull.Value);
                            sqlCommand.Parameters.AddWithValue("@pReaReconditionCost", financeLease.ReaReconditionCost ?? (object)DBNull.Value);
                            sqlCommand.Parameters.AddWithValue("@pAPLeasingMigration", financeLease.APLeasingMigration ?? (object)DBNull.Value);
                            sqlCommand.Parameters.AddWithValue("@pPrepaidMigration", financeLease.PrepaidMigration ?? (object)DBNull.Value);


                            //TODO : Change the CreatedBy and CreatedDate after finished 
                            sqlCommand.Parameters.AddWithValue("@pCreatedBy", financeLease.CreatedBy);
                            sqlCommand.Parameters.AddWithValue("@pCreatedDate", DateTime.Now);
                            sqlCommand.Parameters.AddWithValue("@pModifiedBy", financeLease.ModifiedBy);
                            sqlCommand.Parameters.AddWithValue("@pModifiedDate", DateTime.Now);
                            sqlCommand.Parameters.AddWithValue("@pIsActive", financeLease.IsActive);


                            sqlCommand.ExecuteReader();
                            result.Messages.Add(string.Format("Table {0} has been successfully Updated!", "Finance Lease"));
                            result.IsSuccess = true;

                            if (financeLease.PaymentScheduleList != null)
                            {
                                IList<string> paymentScheduleIDs = GetPaymentScheduleByFinanceLeaseID(financeLease.ID, sqlConnection).Select(x => x.ID).ToList();
                                IList<string> diffList = financeLease.PaymentScheduleList.Where(x => paymentScheduleIDs.Contains(x.ID)).Select(x => x.ID).ToList();

                                // Payment schedule have modification on its member
                                if (financeLease.PaymentScheduleList.Count() != paymentScheduleIDs.Count())
                                {
                                    // delete 
                                    query = "delete FinancePaymentSchedule " +
                                            "where FinanceLeaseID = @pFinanceLeaseID";
                                    sqlCommand.ExecuteReader();
                                    // create
                                    query = "insert into FinancePaymentSchedule" +
                                            "(ID,Term,FinanceLeaseID,PaymentDate,PaymentAmount,CreatedBy,CreatedDate, IsActive)" +
                                            "values" +
                                            "(@pID,@pTerm,@pFinanceLeaseID,@pPaymentDate,@pPaymentAmount,@pCreatedBy,@pCreatedDate,@pIsActive)";
                                    foreach (PaymentSchedule item in financeLease.PaymentScheduleList)
                                    {
                                        using (SqlCommand sqlCommand1 = new SqlCommand(query, sqlConnection))
                                        {
                                            sqlCommand1.CommandType = System.Data.CommandType.Text;
                                            sqlCommand1.Parameters.AddWithValue("@pID", Guid.NewGuid().ToString());
                                            sqlCommand1.Parameters.AddWithValue("@pTerm", item.Term);
                                            sqlCommand1.Parameters.AddWithValue("@pFinanceLeaseID", financeLease.ID);
                                            sqlCommand1.Parameters.AddWithValue("@pPaymentDate", item.PaymentDate);
                                            sqlCommand1.Parameters.AddWithValue("@pPaymentAmount", item.PaymentAmount);

                                            //TODO : Change the CreatedBy and CreatedDate after finished 
                                            sqlCommand1.Parameters.AddWithValue("@pCreatedBy", financeLease.CreatedBy);
                                            sqlCommand1.Parameters.AddWithValue("@pCreatedDate", DateTime.Now);
                                            sqlCommand1.Parameters.AddWithValue("@pIsActive", financeLease.IsActive);

                                            sqlCommand1.ExecuteReader();
                                            result.Messages.Add(string.Format("Table {0} has been successfully inserted!", "Finance Lease"));
                                            result.IsSuccess = true;
                                        }
                                    }

                                }

                                // Payment Schedule is the same as submit
                                else
                                {
                                    query = "update FinancePaymentSchedule set " +
                                           "Term = @pTerm ,PaymentDate = @pPaymentDate ,PaymentAmount = @pPaymentAmount , " +
                                           "ModifiedBy = @pModifiedBy, ModifiedDate = @pModifiedDate, IsActive = @pIsActive " +
                                           "where ID = @pID";

                                    foreach (PaymentSchedule item in financeLease.PaymentScheduleList)
                                    {
                                        using (SqlCommand sqlCommand1 = new SqlCommand(query, sqlConnection))
                                        {
                                            sqlCommand1.CommandType = System.Data.CommandType.Text;
                                            sqlCommand1.Parameters.AddWithValue("@pID", item.ID);
                                            sqlCommand1.Parameters.AddWithValue("@pTerm", item.Term);
                                            sqlCommand1.Parameters.AddWithValue("@pPaymentDate", item.PaymentDate);
                                            sqlCommand1.Parameters.AddWithValue("@pPaymentAmount", item.PaymentAmount);

                                            //TODO : Change the CreatedBy and CreatedDate after finished 
                                            sqlCommand1.Parameters.AddWithValue("@pModifiedBy", financeLease.ModifiedBy);
                                            sqlCommand1.Parameters.AddWithValue("@pModifiedDate", DateTime.Now);
                                            sqlCommand1.Parameters.AddWithValue("@pIsActive", 1);

                                            sqlCommand1.ExecuteReader();
                                            result.Messages.Add(string.Format("Table {0} has been successfully inserted!", "Finance Lease"));
                                            result.IsSuccess = true;
                                        }
                                    }
                                }                             
                            }

                        }
                        sqlConnection.Close();
                    }
                    scope.Complete();
                }
            }

            catch (Exception exception)
            {
                result.Messages.Add(exception.Message);
                result.IsSuccess = false;
            }

            return result;
        }

        public ResultStatus DeleteFinanceLeaseByID(string ID)
        {
            ResultStatus result = new ResultStatus();

            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    using (SqlConnection sqlConnection = new SqlConnection(_connectionString))
                    {
                        sqlConnection.Open();
                        string query = "delete FinanceLease " +
                            "where ID=@pID";

                        using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
                        {
                            sqlCommand.CommandType = System.Data.CommandType.Text;
                            sqlCommand.Parameters.AddWithValue("@pID", ID);
                            //sqlCommand.Parameters.AddWithValue("@pIsActive", 0);

                            sqlCommand.ExecuteReader();
                            result.Messages.Add(string.Format("Table {0} has been successfully Deleted!", "Finance Lease"));
                            result.IsSuccess = true;

                        }

                        query = "delete FinancePaymentSchedule where FinanceLeaseID = @pFinanceLeaseID";
                        using (SqlCommand sqlCommand1 = new SqlCommand(query, sqlConnection))
                        {
                            sqlCommand1.CommandType = System.Data.CommandType.Text;
                            sqlCommand1.Parameters.AddWithValue("@pFinanceLeaseID", ID);

                            //sqlCommand.Parameters.AddWithValue("@pIsActive", 0);

                            sqlCommand1.ExecuteReader();
                            result.Messages.Add(string.Format("Table {0} has been successfully Deleted!", "Finance Payment Schedule"));
                            result.IsSuccess = true;

                        }

                        sqlConnection.Close();
                    }
                    scope.Complete();
                }
            }

            catch (Exception exception)
            {
                result.Messages.Add(exception.Message);
                result.IsSuccess = false;
            }
            return result;
        }

        public List<KeyValuePair<int, string>> GetDDLCategory()
        {
            List<KeyValuePair<int, string>> ddlCategory = new List<KeyValuePair<int, string>>();

            foreach (var item in Enum.GetValues(typeof(CategoryLease)))
            {
                ddlCategory.Add(new KeyValuePair<int, string>((int)item, item.ToString()));
            };

            // Add default selected
            ddlCategory.Add(new KeyValuePair<int, string>(0, "All"));

            return ddlCategory;
        }

        public List<KeyValuePair<int, string>> GetDDLAccType()
        {
            List<KeyValuePair<int, string>> ddlItem = new List<KeyValuePair<int, string>>();

            foreach (var item in Enum.GetValues(typeof(AccType)))
            {
                //ddlItem.Add(new KeyValuePair<string, string>(((int)item).ToString(), Enum.GetName(typeof(AccType), item)));
                ddlItem.Add(new KeyValuePair<int, string>(((int)item), item.ToString()));
            };

            // Add default selected
            ddlItem.Add(new KeyValuePair<int, string>(0, "All"));

            return ddlItem;
        }

        public List<KeyValuePair<string, string>> GetDDLCurrency()
        {
            List<KeyValuePair<string, string>> ddlItem = new List<KeyValuePair<string, string>>();

            ICurrencyLeaseRepository currRepo = new CurrencyLeaseRepository();
            List<KeyValuePair<string, string>> listCurrency = currRepo.GetAllCurrencyLease().Where(x => x.IsActive).
                Select(a => new KeyValuePair<string, string>(a.CurrencyLeaseCode, a.CurrencyLeaseCode)).ToList();

            // Add default selected
            listCurrency.Add(new KeyValuePair<string, string>("0", "All"));

            return listCurrency;
        }

        public List<KeyValuePair<string, string>> GetDDLTypeLease()
        {
            ITypeLeaseRepository typeRepo = new TypeLeaseRepository();
            ICurrencyLeaseRepository currRepo = new CurrencyLeaseRepository();
            List<KeyValuePair<string, string>> listTypeLease = typeRepo.GetAllTypeLease().Where(x => x.IsActive).
                Select(a => new KeyValuePair<string, string>(a.TypeLeaseCode, a.Description)).ToList();

            // Add default selected
            listTypeLease.Add(new KeyValuePair<string, string>("0", "All"));

            return listTypeLease;
        }

        public List<KeyValuePair<int, string>> GetDDLStatus()
        {
            List<KeyValuePair<int, string>> ddlItem = new List<KeyValuePair<int, string>>();

            foreach (var item in Enum.GetValues(typeof(ContractStatus)))
            {
                //ddlItem.Add(new KeyValuePair<string, string>(((int)item).ToString(), Enum.GetName(typeof(AccType), item)));
                ddlItem.Add(new KeyValuePair<int, string>((int)item, item.ToString()));
            };

            // Add default selected
            ddlItem.Add(new KeyValuePair<int, string>(0, "All"));

            return ddlItem;
        }

        public List<KeyValuePair<int, string>> GetDDLPaymentMethod()
        {
            List<KeyValuePair<int, string>> ddlItem = new List<KeyValuePair<int, string>>();

            foreach (var item in Enum.GetValues(typeof(PaymentMethodLease)))
            {
                //ddlItem.Add(new KeyValuePair<string, string>(((int)item).ToString(), Enum.GetName(typeof(AccType), item)));
                ddlItem.Add(new KeyValuePair<int, string>((int)item, item.ToString()));
            };

            // Add default selected
            ddlItem.Add(new KeyValuePair<int, string>(0, "All"));

            return ddlItem;
        }

        private string GetRefNo(FinanceLease financeLease, SqlConnection sqlConnection)
        {
            string refNo =  financeLease.CategoryLeaseID == (int?)CategoryLease.Draft ? "DRFT" : "RENT";
            string lastRefNo = "";
            int number = 1;
            string monthDate = (DateTime.Now.Month.ToString()).PadLeft(2, '0');
            string yearDate = (DateTime.Now.Year.ToString());
            yearDate = yearDate[2].ToString() + yearDate[3].ToString();
            //string type = financeLease.TypeLeaseCode;
            //string accType = financeLease.AccTypeID;

            string query = "select top(1) RefNo from FinanceLease where RefNo like @pRefNo and len(RefNo) = 21 order by CreatedDate desc";

            using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
            {
                sqlCommand.CommandType = System.Data.CommandType.Text;
                sqlCommand.Parameters.AddWithValue("@pRefNo", refNo + "%");

                using (SqlDataReader dataReader = sqlCommand.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        lastRefNo = (dataReader["RefNo"].ToString());
                    }
                }
            }


            if (!string.IsNullOrEmpty(lastRefNo))
            {
                List<string> listString = lastRefNo.Split('/').ToList();

                if (listString[3] == monthDate && listString[4] == yearDate)
                {
                    int.TryParse(lastRefNo.Split('/')[5], out number);
                    number++;
                }
            }

            //typelaseid
            refNo += "/" + financeLease.TypeLeaseCode + "/";

            if (financeLease.AccTypeID == (int?)AccType.Conventional) refNo += "CO/";
            if (financeLease.AccTypeID == (int?)AccType.Sharia) refNo += "SY/";

            refNo += monthDate + "/";
            refNo += yearDate + "/";

            refNo += number.ToString().PadLeft(3, '0');

            return refNo;
        }

        public string GetRefNoModified(FinanceLease financeLease)
        {
            string refNo = financeLease.RefNo.Split('-')[0];
            int number = 1;
            if (financeLease.CategoryLeaseID == (int?)CategoryLease.Reverse)
            {
                refNo += "-C00";
                return refNo;
            }

            if (financeLease.CategoryLeaseID == (int?)CategoryLease.Terminate)
            {
                refNo += "-T00";
                return refNo;
            }

            string lastRefNo = "";

            using (SqlConnection sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                string query = "select top(1) RefNo from FinanceLease where RefNo like @pRefNo order by CreatedDate desc";

                using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
                {
                    sqlCommand.CommandType = System.Data.CommandType.Text;
                    sqlCommand.Parameters.AddWithValue("@pRefNo", refNo + "-R%");

                    using (SqlDataReader dataReader = sqlCommand.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            lastRefNo = (dataReader["RefNo"].ToString());
                        }
                    }
                }
                sqlConnection.Close();
            }

            if (!string.IsNullOrEmpty(lastRefNo))
            {
                int.TryParse((lastRefNo.Split('-')[1]).TrimStart('R'), out number);
                number++; 
            }

            refNo += "-R" + number.ToString().PadLeft(2, '0');

            return refNo;
        }

        private ResultStatus CreateActionHistory(FinanceLease financeLease, SqlConnection sqlConnection, string newID = null)
        {
            ResultStatus retStatus = new ResultStatus();

            string query = "insert into ActionHistory" +
                "(ID, NPK, FinanceLeaseID, ContractActionID, Remarks, CreatedBy, CreatedDate, IsActive)" +
                "values" +
                "(@pID,@pNPK, @pFinanceLeaseID, @pContractActionID, @pRemarks, @pCreatedBy, @pCreatedDate, @pIsActive)";

            using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
            {
                sqlCommand.CommandType = System.Data.CommandType.Text;
                sqlCommand.Parameters.AddWithValue("@pID", Guid.NewGuid().ToString());

                // Change this value when its ready

                sqlCommand.Parameters.AddWithValue("@pFinanceLeaseID", newID ?? financeLease.ID);
                sqlCommand.Parameters.AddWithValue("@pContractActionID", financeLease.StatusLeaseID);
                sqlCommand.Parameters.AddWithValue("@pRemarks", financeLease.ActionHistoryList[0].Remarks);

                sqlCommand.Parameters.AddWithValue("@pNPK", "1111111");

                sqlCommand.Parameters.AddWithValue("@pCreatedBy", financeLease.CreatedBy);
                sqlCommand.Parameters.AddWithValue("@pCreatedDate", DateTime.Now);
                sqlCommand.Parameters.AddWithValue("@pIsActive", 1);

                sqlCommand.ExecuteReader();
                retStatus.Messages.Add(string.Format("Table {0} has been successfully inserted!", "Action History"));
                retStatus.IsSuccess = true;

            }
            return retStatus;
        }

        public ResultStatus UpdateStatusFinanceLease(FinanceLease financeLease)
        {
            ResultStatus result = new ResultStatus();
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    using (SqlConnection sqlConnection = new SqlConnection(_connectionString))
                    {
                        sqlConnection.Open();
                        string query = "update FinanceLease set" +
                            " StatusLeaseID = @pStatusLeaseID, ModifiedBy = @pModifiedBy, ModifiedDate = @pModifiedDate " +
                            "where ID=@pID";

                        using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
                        {
                            sqlCommand.CommandType = System.Data.CommandType.Text;
                            sqlCommand.Parameters.AddWithValue("@pID", financeLease.ID.ToString());
                            sqlCommand.Parameters.AddWithValue("@pStatusLeaseID", financeLease.StatusLeaseID);


                            //TODO : Change the CreatedBy and CreatedDate after finished 
                            sqlCommand.Parameters.AddWithValue("@pModifiedBy", financeLease.ModifiedBy);
                            sqlCommand.Parameters.AddWithValue("@pModifiedDate", DateTime.Now);

                            sqlCommand.ExecuteReader();
                            result.Messages.Add(string.Format("Table {0} has been successfully Updated!", "Finance Lease"));
                            result.IsSuccess = true;
                        }

                        result.Messages.AddRange(CreateActionHistory(financeLease, sqlConnection).Messages);
                        sqlConnection.Close();
                    }
                    scope.Complete();
                }
            }

            catch (Exception exception)
            {
                result.Messages.Add(exception.Message);
                result.IsSuccess = false;
            }
            return result;
        }

        public ResultStatus UpdateStatusActiveFinanceLease(FinanceLease financeLease)
        {
            ResultStatus result = new ResultStatus();
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    using (SqlConnection sqlConnection = new SqlConnection(_connectionString))
                    {
                        sqlConnection.Open();
                        string query = "update FinanceLease set" +
                            " IsActive = @pIsActive, ModifiedBy = @pModifiedBy, ModifiedDate = @pModifiedDate " +
                            "where ID=@pID";

                        using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
                        {
                            sqlCommand.CommandType = System.Data.CommandType.Text;
                            sqlCommand.Parameters.AddWithValue("@pID", financeLease.ID.ToString());

                            sqlCommand.Parameters.AddWithValue("@pModifiedBy", financeLease.ModifiedBy);
                            sqlCommand.Parameters.AddWithValue("@pModifiedDate", DateTime.Now);
                            sqlCommand.Parameters.AddWithValue("@pIsActive", financeLease.IsActive);

                            sqlCommand.ExecuteReader();
                            result.Messages.Add(string.Format("Table {0} has been successfully Updated!", "Finance Lease"));
                            result.IsSuccess = true;
                        }

                        sqlConnection.Close();
                    }
                    scope.Complete();
                }
            }

            catch (Exception exception)
            {
                result.Messages.Add(exception.Message);
                result.IsSuccess = false;
            }
            return result;
        }

        public ResultStatus UpdateRealisationRecondition(FinanceLease financeLease)
        {
            ResultStatus result = new ResultStatus();
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    using (SqlConnection sqlConnection = new SqlConnection(_connectionString))
                    {
                        sqlConnection.Open();
                        string query = "update FinanceLease set" +
                            " ReaReconditionCost = @pReaReconditionCost, ModifiedBy = @pModifiedBy, ModifiedDate = @pModifiedDate " +
                            "where ID=@pID";

                        using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
                        {
                            sqlCommand.CommandType = System.Data.CommandType.Text;
                            sqlCommand.Parameters.AddWithValue("@pID", financeLease.ID.ToString());
                            sqlCommand.Parameters.AddWithValue("@pReaReconditionCost", financeLease.ReaReconditionCost);
                            sqlCommand.Parameters.AddWithValue("@pModifiedBy", financeLease.ModifiedBy);
                            sqlCommand.Parameters.AddWithValue("@pModifiedDate", DateTime.Now);

                            sqlCommand.ExecuteReader();
                            result.Messages.Add(string.Format("Table {0} has been successfully Updated!", "Finance Lease"));
                            result.IsSuccess = true;
                        }

                        result.Messages.AddRange(CreateActionHistory(financeLease, sqlConnection).Messages);
                        sqlConnection.Close();
                    }
                    scope.Complete();
                }
            }

            catch (Exception exception)
            {
                result.Messages.Add(exception.Message);
                result.IsSuccess = false;
            }
            return result;
        }

        public List<PaymentSchedule> GetPaymentScheduleByFinanceLeaseID(string ID)
        {
            List<PaymentSchedule> paymentScheduleList = new List<PaymentSchedule>();
            using (SqlConnection sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                paymentScheduleList.AddRange(GetPaymentScheduleByFinanceLeaseID(ID, sqlConnection));
                sqlConnection.Close();
            }

            return paymentScheduleList;
        }

        private List<PaymentSchedule> GetPaymentScheduleByFinanceLeaseID(string ID, SqlConnection sqlConnection)
        {
            List<PaymentSchedule> paymentScheduleList = new List<PaymentSchedule>();
            string query = "select * from FinancePaymentSchedule where FinanceLeaseID = @pFinanceLeaseID";
            using (SqlCommand sqlCommand1 = new SqlCommand(query, sqlConnection))
            {
                sqlCommand1.CommandType = System.Data.CommandType.Text;
                sqlCommand1.Parameters.AddWithValue("@pFinanceLeaseID", ID);

                using (SqlDataReader dataReader = sqlCommand1.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        paymentScheduleList.Add
                            (new PaymentSchedule
                            {
                                ID = dataReader["ID"].ToString(),
                                Term = (int)dataReader["Term"],
                                PaymentAmount = (double)(decimal)dataReader["PaymentAmount"],
                                PaymentDate = (DateTime)dataReader["PaymentDate"]
                            });
                    }
                }
            }
            return paymentScheduleList;
        }
    }
}
