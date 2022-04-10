using Proviser.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Proviser.Data
{
    public class DataBase
    {
        readonly SQLiteAsyncConnection _courtsDataBase;
        readonly SQLiteAsyncConnection _casesDataBase;
        readonly SQLiteAsyncConnection _decisionDataBase;

        public DataBase(string _connectionString, List<string> _dataBaseName)
        {
            _courtsDataBase = new SQLiteAsyncConnection(Path.Combine(_connectionString, _dataBaseName[0]));
            _courtsDataBase.CreateTableAsync<Courts>().Wait();

            _casesDataBase = new SQLiteAsyncConnection(Path.Combine(_connectionString, _dataBaseName[1]));
            _casesDataBase.CreateTableAsync<Cases>().Wait();

            _decisionDataBase = new SQLiteAsyncConnection(Path.Combine(_connectionString, _dataBaseName[2]));
            _decisionDataBase.CreateTableAsync<Decision>().Wait();
        }


        #region Decisions

        public Task<int> SaveDecisionAsync(Decision _decision)
        {
            try
            {
                return _decisionDataBase.InsertAsync(_decision);
            }
            catch
            {
                return null;
            }

        }

        public Task<List<Decision>> GetDecisionByCaseAsync(string _case)
        {
            return _decisionDataBase.Table<Decision>().Where(x => x.Case == _case).ToListAsync();
        }


        #endregion

        #region Courts

        public Task<int> SaveCourtsAsync(Courts _courts)
        {
            try
            {
                return _courtsDataBase.InsertAsync(_courts);
            }
            catch
            {
                return null;
            }

        }

        public Task<int> DeleteCourtsAsync(Courts _courts)
        {
            try
            {
                return _courtsDataBase.DeleteAsync(_courts);
            }
            catch
            {
                return null;
            }

        }

        public Task<int> UpdateCourtsAsync(Courts _courts)
        {
            try
            {
                return _courtsDataBase.UpdateAsync(_courts);
            }
            catch
            {
                return null;
            }

        }

        public Task<List<Courts>> GetCourtsAsync()
        {
            return _courtsDataBase.Table<Courts>().ToListAsync();
        }

        public Task<Courts> GetCourtsAsync(int _id)
        {
            return _courtsDataBase.Table<Courts>().Where(x => x.Id == _id).FirstOrDefaultAsync();
        }

        public Task<List<Courts>> GetCourtsAsync(string _case)
        {
            return _courtsDataBase.Table<Courts>().Where(x => x.Case == _case).ToListAsync();
        }

        public Task<List<Courts>> GetCourtsByLittigansAsync(string _value)
        {
            return _courtsDataBase.Table<Courts>().Where(x => x.Littigans.Contains(_value)).ToListAsync();
        }

        #endregion

        #region Cases

        public Task<int> SaveCasesAsync(Cases _cases)
        {
            try
            {
                return _casesDataBase.InsertAsync(_cases);
            }
            catch
            {
                return null;
            }

        }

        public Task<int> DeleteCasesAsync(Cases _cases)
        {
            try
            {
                return _casesDataBase.DeleteAsync(_cases);
            }
            catch
            {
                return null;
            }

        }

        public Task<int> UpdateCasesAsync(Cases _cases)
        {
            try
            {
                return _casesDataBase.UpdateAsync(_cases);
            }
            catch
            {
                return null;
            }

        }

        public Task<List<Cases>> GetCasesAsync()
        {
            return _casesDataBase.Table<Cases>().ToListAsync();
        }

        public Task<Cases> GetCasesAsync(int _id)
        {
            return _casesDataBase.Table<Cases>().Where(x => x.Id == _id).FirstOrDefaultAsync();
        }

        public Task<Cases> GetCasesByCaseAsync(string _case)
        {
            return _casesDataBase.Table<Cases>().Where(x => x.Case == _case).FirstOrDefaultAsync();
        }

        #endregion

        #region Functions

        public async Task<List<Courts>> GetCourtsHearingOrderingByDateAsync()
        {
            List<Courts> _courts = new List<Courts>();
            List<Courts> _result = new List<Courts>();

            var _cases = await GetCasesAsync();
            if (_cases.Count > 0)
            {
                foreach (var _case in _cases)
                {
                    var _list = await GetCourtsAsync(_case.Case);
                    if (_list.Count > 0)
                    {
                        _courts.AddRange(_list);
                    }
                }

                _courts = _courts.OrderBy(x => x.Date).ToList();

                if (_courts.Count > 0)
                {
                    foreach (var _item in _courts)
                    {
                        if (_item.Date >= DateTime.Today)
                        {
                            _result.Add(_item);
                        }
                    }

                    return _result;
                }
                else
                {
                    return null;
                }

            }
            else
            {
                return null;
            }

        }

        #endregion
    }
}
