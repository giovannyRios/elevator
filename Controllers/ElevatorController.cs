using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Elevator.Logic;
using Elevator.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Elevator.Models;
using System.Net;
using Microsoft.Extensions.Logging;

namespace Elevator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ElevatorController : ControllerBase
    {
        private readonly IOptions<Jwt_values> _options;
        private readonly ILogger<ElevatorController> _logger;

        public ElevatorController(ILogger<ElevatorController> logger, IOptions<Jwt_values> options)
        {
            _options = options;
            _logger = logger;
        }

        private void registrarLog(string mensaje)
        {
            _logger.LogInformation(mensaje);
        } 


        /// <summary>
        /// Method that allows indicating the floor to which the user is going from inside the elevator
        /// </summary>
        /// <returns>
        /// IActionResult with http status code
        /// </returns>
        [Authorize]
        [HttpGet]
        [Route("~/api/[controller]/GetElevatorInside/{id}")]
        public IActionResult GetElevatorInside(int id)
        {

            try
            {
                var objJWT = _options.Value;
                ElevatorClass objElevator = ElevatorClass.getInstance("", objJWT);

                if (!objElevator.TryRetrieveToken(Request, out string token))
                {
                    registrarLog("token invalido");
                    return Unauthorized("token invalido");
                }
                else
                {
                    if (objElevator.ValidateToken(objElevator.CheckToken(), objJWT) && objElevator.ValidateToken(token, objJWT)) 
                    {
                        if (id > 15 || id < 1)
                        {
                            registrarLog("Piso no autorizado");
                            return Unauthorized("Piso no autorizado");
                        }
                        else
                        {
                            registrarLog(objElevator.FloorPress(id));
                            return Ok(objElevator.FloorPress(id));
                        }
                    }
                    else
                    {
                        registrarLog("El ascensor tiene inconveniente, oprima 'turnOnElevator'");
                        return StatusCode(500, "El ascensor tiene inconveniente, oprima 'turnOnElevator'");
                    }
                }
            }
            catch (Exception e)
            {
                registrarLog($"Error, verifique: {e.ToString()}");
                return StatusCode(500);
            }
        }

        /// <summary>
        /// Method that allows indicating the floor to which the user is going from inside the elevator
        /// </summary>
        /// <returns>
        /// IActionResult with http status code
        /// </returns>
        [Authorize]
        [HttpPost(Name = "GetElevatorOutside"), Route("~/api/[controller]/GetElevatorOutside")]
        public IActionResult GetElevatorOutside(floor objFloor)
        {
            try
            {
                var objJWT = _options.Value;
                ElevatorClass objElevator = ElevatorClass.getInstance("", objJWT);

                if (!objElevator.TryRetrieveToken(Request, out string token))
                {
                    registrarLog("token invalido");
                    return Unauthorized("token invalido");
                }
                else
                {
                    if (objElevator.ValidateToken(objElevator.CheckToken(), objJWT) && objElevator.ValidateToken(token, objJWT))
                    {
                        string mensaje = null;
                        List<string> lResult = new List<string>();
                        mensaje = objElevator.FloorPress(objFloor.currentFloor);
                        string temp = mensaje.Replace("El piso actual donde esta el ascensor es", "El piso donde estaba el ascensor es");
                        registrarLog(temp);
                        lResult.Add(temp);
                        mensaje = objElevator.FloorPress(objFloor.destinationFloor);
                        registrarLog(mensaje);
                        lResult.Add(mensaje);
                        return Ok(lResult.ToArray());
                    }
                    else
                    {
                        registrarLog("El ascensor tiene inconveniente, oprima 'turnOnElevator'");
                        return StatusCode(500, "El ascensor tiene inconveniente, oprima 'turnOnElevator'");
                    }
                }
            }
            catch (Exception e)
            {
                registrarLog($"Error, verifique: {e.ToString()}");
                return StatusCode(500);
            }
        }


        /// <summary>
        /// Method that turns on the elevator and starts the authorization process
        /// </summary>
        /// <param name="userName">Name that will remain associated with the elevator instance</param>
        /// <returns>
        /// ActionResult with http status code 
        /// </returns>
        [HttpPost(Name = "turnOnElevator"), Route("~/api/[controller]/turnOnElevator")]
        public IActionResult turnOnElevator([FromBody] user userName)
        {
            try
            {
                var objJWT = _options.Value;
                ElevatorClass objElevator = ElevatorClass.getInstance(userName.userName, objJWT);

                switch (objElevator.CheckStatus())
                {
                    case HttpStatusCode.Continue: 

                        Request.HttpContext.Response.Headers.Add("token", objElevator.value);
                        registrarLog($"El ascensor ya se encontraba operando, token de acceso: {objElevator.value}");
                        return Ok(new string[] { "El ascensor ya se encontraba operando", objElevator.value});

                    case HttpStatusCode.OK:

                        registrarLog($"ascensor encendido, token: {objElevator.value}");
                        Request.HttpContext.Response.Headers.Add("token", objElevator.value);
                        return Ok(new string[] {"ascensor encendido", string.Concat("token: ", objElevator.value)});

                    case HttpStatusCode.Unauthorized:

                        Request.HttpContext.Response.Headers.Clear();
                        registrarLog("Acceso no autorizado");
                        return Unauthorized("Acceso no autorizado");

                    default:

                        Request.HttpContext.Response.Headers.Clear();
                        registrarLog("Recurso no encontrado");
                        return NoContent();

                }

            }
            catch (Exception e)
            {
                registrarLog($"Error, verifique: {e.ToString()}");
                return StatusCode(500);
            }
        }


        /// <summary>
        /// Method that restarts the elevator instance, after calling this method the turnOnElevator method must be called
        /// </summary>
        /// <returns>IactionResult</returns>
        [Authorize]
        [HttpPut(Name = "reloadInstance"),Route("~/api/[controller]/reloadInstance")]
        public IActionResult reloadInstance()
        {
            var objJWT = _options.Value;
            Request.HttpContext.Request.Headers.Clear();
            ElevatorClass objElevator = ElevatorClass.getInstance("", objJWT);
            objElevator.reloadInstance();
            registrarLog("Instancia del elevador reiniciada, recuerde que tiene que llamar nuevamente el recurso turnOnElevator");
            return Ok("reload success");
        }

    }
}
