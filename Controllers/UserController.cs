using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;
using System.Data;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ApiLibros.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private IConfiguration Configuration;
        private readonly string _connectionString;


        public UserController(IConfiguration _configuration)
        {
            Configuration = _configuration;
            _connectionString = _configuration.GetConnectionString("mysqlConnection");
        }




        [HttpGet]
        [Produces("application/json")]
        [Route("all")]
        public IActionResult All()
        {

            try
            {
                using (MySqlConnection conn = new MySqlConnection(_connectionString))
                {
                    using (MySqlCommand cmd = new MySqlCommand("sp_person", conn))
                    {
                        
                        dynamic objetos = null;

                        JArray listaObjetos = null;

                        cmd.CommandType = CommandType.StoredProcedure;
                        conn.Open();

                        cmd.Parameters.AddWithValue("@first_name", null);
                        cmd.Parameters.AddWithValue("@last_name", null);
                        cmd.Parameters.AddWithValue("@birth_date", null);
                        cmd.Parameters.AddWithValue("@gender", null);
                        cmd.Parameters.AddWithValue("@status", null);
                        cmd.Parameters.AddWithValue("@edit_date", null);
                        cmd.Parameters.AddWithValue("@option_control", 1);

                        MySqlDataReader reader = cmd.ExecuteReader();

                        if (reader.HasRows)
                        {
                            listaObjetos = new JArray();

                            while (reader.Read())
                            {
                                objetos = new JObject();
                                objetos.id = Int32.Parse(reader["id"].ToString());
                                objetos.nombreCompleto = reader["first_name"].ToString() + " " + reader["last_name"].ToString();
                                objetos.fecha_nacimiento = reader["birth_date"].ToString();
                                objetos.genero = reader["gender"].ToString();
                                objetos.estado = Int32.Parse(reader["status"].ToString());
                                listaObjetos.Add(objetos);
                            }
                            return Ok(new { response = listaObjetos });
                        }
                        
                        JArray vacio = new JArray();
                        return Ok(new { response = vacio });
                    }
                }
            }
            catch (Exception ex)
            {
                dynamic data = new JObject();
                data.value = ex.ToString();
                data.response = 6;
                data.message = "Proceso no realizado, excepcion";
                return BadRequest(data);
            }
        }




        [HttpPost]
        [Produces("application/json")]
        [Route("insert_person")]
        public IActionResult InsertarUsuario(JObject request)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(_connectionString))
                {
                    using (MySqlCommand cmd = new MySqlCommand("sp_person", conn))
                    {

                        dynamic data;
                        cmd.CommandType = CommandType.StoredProcedure;
                        //conn.Open();

                        cmd.Parameters.AddWithValue("@first_name", request.GetValue("nombre").ToString());
                        cmd.Parameters.AddWithValue("@last_name", request.GetValue("apellido").ToString());
                        cmd.Parameters.AddWithValue("@birth_date", null);
                        cmd.Parameters.AddWithValue("@gender", Int32.Parse(request.GetValue("genero").ToString()));
                        cmd.Parameters.AddWithValue("@status", null);
                        cmd.Parameters.AddWithValue("@edit_date", null);
                        cmd.Parameters.AddWithValue("@option_control", 2);


                        conn.Open();
                        DataSet setter = new DataSet();
                        MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                        adapter.Fill(setter, "documento");

                        if (setter.Tables["documento"].Rows.Count > 0 || setter != null)
                        {
                            data = new JObject();
                            data.response = 4;
                            data.message = "Registro Guardado Exitosamente";
                            return Ok(data);
                        }
                        else
                        {
                            data = new JObject();
                            data.value = 0;
                            data.response = 4;
                            data.message = setter.Tables[0].Rows[0][0];
                            return BadRequest(data);
                        }



                    }
                }
            }
            catch (Exception ex)
            {
                dynamic data = new JObject();
                data.value = ex.ToString();
                data.response = 6;
                data.message = "Proceso no realizado, excepcion";
                return BadRequest(data);
            }

        }


















            // GET: api/<UserController>
            [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<UserController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<UserController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<UserController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<UserController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
