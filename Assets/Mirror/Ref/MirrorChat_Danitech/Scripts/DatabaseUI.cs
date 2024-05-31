using UnityEngine;
using UnityEngine.UI;
using MySql.Data.MySqlClient;
using System;
using System.Data;

public class DatabaseUI : MonoBehaviour
{
   [Header("UI")]
   [SerializeField] InputField Input_Query;
   [SerializeField] Text Text_DBResult;
   [SerializeField] Text Text_Log;

    [Header("Connection Info")]
    [SerializeField] string _ip = "127.0.0.1";
    [SerializeField] string _dbName = "test";
    [SerializeField] string _uid = "root";
    [SerializeField] string _pwd = "1234";


    private bool _isConnectTestComplete;

    private static MySqlConnection _dbConnection;

    private void Awake()
    {
        this.gameObject.SetActive(false);
    }

    private void SendQuery(string queryStr, string tableName)
    {
        //있으면 Select 관연 함수 호출
        if (queryStr.Contains("SELECT"))
        {
            DataSet dataSet = OnSelectReQuest(queryStr, tableName);
            Text_DBResult.text = DeformatResult(dataSet);
        }
        else//없으면 Insert 또는 Update 관련 쿼리
        {
            Text_DBResult.text = OnInsertOnUpdateRequst(queryStr) ? "성공" : "실패";


        }

        
    }

    public static bool OnInsertOnUpdateRequst(string queryStr)
    {
        try
        {
            MySqlCommand sqlCommand = new MySqlCommand();
            sqlCommand.Connection = _dbConnection;
            sqlCommand.CommandText = queryStr;

            _dbConnection.Open();
            sqlCommand.ExecuteNonQuery();

            _dbConnection.Close();
            return true;
        }
        catch 
        {
            return false;
        }
    }


    private string DeformatResult(DataSet dataSet)
    {
        string resultStr = string.Empty;

        foreach (DataTable table in dataSet.Tables)
        {
            foreach (DataRow row in table.Rows)
            {
                foreach (DataColumn column in table.Columns)
                {
                    resultStr += $"{column.ColumnName}:{row[column]}\n";
                }
            }
        }

        return resultStr;
    }

    public static DataSet OnSelectReQuest(string query,string tableName)
    {
        try
        {
            _dbConnection.Open();
            MySqlCommand sqlCmd=new MySqlCommand();
            sqlCmd.Connection = _dbConnection;
            sqlCmd.CommandText = query;

            MySqlDataAdapter sd = new MySqlDataAdapter(sqlCmd);
            DataSet dataset = new DataSet();
            sd.Fill(dataset, tableName);

            _dbConnection.Close();
            return dataset;
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
            return null;
        }
    }


    private bool ConnectTest()
    {
        string connectStr=$"Server={_ip};Database={_dbName};Uid={_uid};Pwd={_pwd};";

        try
        {
            using(MySqlConnection conn= new MySqlConnection(connectStr))
            {
                _dbConnection = conn;
                conn.Open();
            }

            Text_Log.text = "DB 연결을 성공했습니다 ";

            return true;
        }
        catch (Exception e)
        {
            Debug.LogWarning($"e:{e.ToString()}");
            Text_Log.text = "DB 연결 실패";
            return false;

        }
    }

    public void OnClick_TestDBConnect()
    {
        _isConnectTestComplete = ConnectTest();
    }

    public void OnSubmit_SendQuery()
    {
        if (!_isConnectTestComplete)
        {
            Text_Log.text = "DB연결을 먼저 해주세요";
        }
        Text_Log.text = string.Empty;

        string query = string.IsNullOrWhiteSpace(Input_Query.text) ? "SELECT U_PlayerId, U_NickName, U_Level, U_LastQuestId FROM game_player" :
        Input_Query.text;

        SendQuery(query, "user_info");
       
    }

    public void OnClick_OpenDatabaseUI()
    {
        this.gameObject.SetActive(true);
    }

    public void OnClick_CloseDatabaseUI()
    {
        this.gameObject.SetActive(false);
    }

}
