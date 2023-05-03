using System;
using System.Text.Json;
using System.Text.Json.Serialization;

        ///<summary>
        /// a Class <c>Response</c> represents the result of a call to a void function. 
        ///If an exception was thrown: <c>ErrorOccured = true</c> and <c>ErrorMessage != null</c>. 
        ///Otherwise, <c>ErrorOccured = false</c> and <c>ErrorMessage = null</c>.
        /// </summary>
public class Response<T>
{

    
    public string ErrorMessage { get; set;}

    public T ReturnValue { get; set;}



    public Response(string returnErr, T returnVal)
    {
        this.ErrorMessage = returnErr;
        this.ReturnValue = returnVal;
    }



    internal  string toJson()
    {
        if (ErrorMessage == null || ErrorMessage == "")
            ErrorMessage = null;
        if (ReturnValue == null || (ReturnValue).Equals(""))
        {
            ReturnValue = default(T);
        }
            

        //in order to bypass null
        JsonSerializerOptions options = new JsonSerializerOptions();
        options.WriteIndented = true;
        options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        return JsonSerializer.Serialize<Response<T>>(this, options);
    }


    internal bool hasError()
    {
        return (ErrorMessage != null || ErrorMessage.Length != 0);
    }

}