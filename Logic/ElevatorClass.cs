using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using Elevator.Security;
using Microsoft.Extensions.Options;
using Elevator.Models;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Elevator.Logic
{
    public class ElevatorClass
    {

        private static ElevatorClass _instance;
        public string value { get; private set; }
        private Stack<int> stElevatorFloor = null;
        private const int _finalFloor = 15;
        private int _currentFloor = 1;
        private HttpStatusCode state = HttpStatusCode.NotImplemented;
        private static readonly object _lock = new object();
        private ElevatorClass() { }

        /// <summary>
        /// Method that controls the single instance of the elevator class
        /// </summary>
        /// <param name="userName">User associated with the instance</param>
        /// <param name="_options">Values obtained from the configuration file for the JWT security token</param>
        /// <returns>Single instance associated with the elevator class</returns>
        public static ElevatorClass getInstance(string userName, Jwt_values _options)
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new ElevatorClass();
                        _instance.value = TokenGenerator.tokenGeneratorJWT(userName, _options);
                        _instance.state = HttpStatusCode.OK;
                        _instance.stElevatorFloor = new Stack<int>();
                    }
                    else
                    {
                        if (tokenValidation.Validation(_instance.value, _options))
                        {
                            _instance.state = HttpStatusCode.Continue;
                        }
                        else
                        {
                            _instance.state = HttpStatusCode.Unauthorized;
                        }
                    }
                }
            }
            else
            {
                if (tokenValidation.Validation(_instance.value, _options))
                {
                    _instance.state = HttpStatusCode.Continue;
                }
                else
                {
                    _instance.state = HttpStatusCode.Unauthorized;
                }
            }

            return _instance;
        }

        /// <summary>
        /// Method that coordinates the movement of the elevator according to the indicated floor
        /// </summary>
        /// <param name="floor">floor requested by the user</param>
        /// <returns>current floor the elevator is on</returns>
        public string FloorPress(int floor)
        {
            if (floor < 1 || floor > 15)
            {
                return CheckCurrentFloor();
            }
            if (floor - _currentFloor < 0 && _currentFloor > 1)
                down(floor);
            else if (floor - _currentFloor > 0 && _currentFloor < _finalFloor)
            {
                up(floor);
            }
            else
            {
                Stop(floor);
            }
            return CheckCurrentFloor();
        }

        /// <summary>
        /// Allows the elevator to go up
        /// </summary>
        /// <param name="floor">floor requested by the user</param>
        private void up(int floor)
        {
            try
            {
                for (int i = _currentFloor; i <= floor; i++)
                {
                    stElevatorFloor.Push(i);
                    Thread.Sleep(1000);
                }
                Stop(floor);
            }
            catch (Exception ex)
            {
                //TODO: Implement log
                throw ex;
            }
        }

        /// <summary>
        /// Allows the elevator to go down
        /// </summary>
        /// <param name="floor">floor requested by the user</param>
        private void down(int floor)
        {
            try
            {
                for (int i = _currentFloor; i >= floor; i--)
                {
                    _ = stElevatorFloor.Pop();
                    Thread.Sleep(1000);
                }
                Stop(floor);
            }
            catch (Exception ex)
            {
                //TODO: Implement log
                throw ex;
            }
        }

        /// <summary>
        /// stops the elevator
        /// </summary>
        /// <param name="floor">floor requested by the user</param>
        private void Stop(int floor)
        {
            _currentFloor = floor;
        }

        /// <summary>
        /// Return message with the current floor where the elevator is located
        /// </summary>
        /// <returns>message with the current floor where the elevator is located</returns>
        public string CheckCurrentFloor()
        {
            return "El piso actual donde esta el ascensor es: " + Convert.ToString(_currentFloor);
        }

        /// <summary>
        /// returns current http code of the elevator instance
        /// </summary>
        /// <returns>elevator instance http status code</returns>
        public HttpStatusCode CheckStatus()
        {
            return _instance.state;
        }

        /// <summary>
        /// query the JWT token that is associated with the instance of the elevator class
        /// </summary>
        /// <returns>Token JWT</returns>
        public string CheckToken()
        {
            return _instance.value;
        }

        /// <summary>
        /// Restart elevator instance, warning: after this action the elevator instance is null
        /// </summary>
        public void reloadInstance()
        {
            _instance = null;
        }

        /// <summary>
        /// Validate the validity of the JWT security token
        /// </summary>
        /// <param name="token">token to validate</param>
        /// <param name="_options">Values obtained from the configuration file for the JWT security token</param>
        /// <returns>false or true according to the correct validation of the security token</returns>
        public bool ValidateToken(string token, Jwt_values _options)
        {
            return tokenValidation.Validation(token, _options);
        }

        /// <summary>
        /// Get the security token assigned in the httpRequest
        /// </summary>
        /// <param name="request">current HttpRequest</param>
        /// <param name="token">output parameter that receives the original token</param>
        /// <returns>true or false indicates whether or not the security token could be retrieved from the current http request</returns>
        public bool TryRetrieveToken(HttpRequest request, out string token)
        {
            try
            {
                token = null;
                if (!request.Headers.TryGetValue("Authorization", out StringValues value) || StringValues.IsNullOrEmpty(value))
                {
                    return false;
                }

                var bearerToken = value.ElementAt(0);

                token = bearerToken.StartsWith("Bearer ") ? bearerToken.Substring(7) : bearerToken;

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
