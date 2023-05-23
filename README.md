# Import CSV in ASP.NET Core

In this  solution we are implementing importing csv data functionality  in an Web API. 
The InputFormatter  is used to convert the csv data to the C# model classes. 



The CDR class is used as the model class to import from csv data.

```csharp
using System;

namespace AspNetCoreCsvImport.Model
{
      public class CDR
    {
        public int caller_id { get; set; }
        public string recipient { get; set; }
        public DateTime call_date { get; set; }
        public DateTime end_time { get; set; }
        public int duration { get; set; }
        public double cost { get; set; }
        public string reference { get; set; }
        public double currency { get; set; }

    }
}
```

The MVC Controller CsvTestController  makes it possible to import  the data. 

        // POST api/csvtest/import
        [HttpPost]
        [Route("import")]
        public IActionResult Import([FromBody]List<LocalizationRecord> value)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                List<LocalizationRecord> data = value;
                return Ok();
            }
        }

    }
}

```

The csv input formatter implements the InputFormatter class. This checks if the context ModelType property is a type of IList and if so, converts the csv data to a List of Objects of type T using reflection. This is implemented in the read stream method. The implementation is very basic and will not work if you have more complex structures in your model class.

 
```csharp
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using System.Text;

namespace AspNetCoreCsvImport.Formatters
{
    /// <summary>
    /// ContentType: text/csv
    /// </summary>
    public class CsvInputFormatter : InputFormatter
    {
        private readonly CsvFormatterOptions _options;

        public CsvInputFormatter(CsvFormatterOptions csvFormatterOptions)
        {
            SupportedMediaTypes.Add(Microsoft.Net.Http.Headers.MediaTypeHeaderValue.Parse("text/csv"));

            if (csvFormatterOptions == null)
            {
                throw new ArgumentNullException(nameof(csvFormatterOptions));
            }

            _options = csvFormatterOptions;
        }

        public override Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context)
        {
            var type = context.ModelType;
            var request = context.HttpContext.Request;
            MediaTypeHeaderValue requestContentType = null;
            MediaTypeHeaderValue.TryParse(request.ContentType, out requestContentType);


            var result = ReadStream(type, request.Body);
            return InputFormatterResult.SuccessAsync(result);
        }

        public override bool CanRead(InputFormatterContext context)
        {
            var type = context.ModelType;
            if (type == null)
                throw new ArgumentNullException("type");

            return IsTypeOfIEnumerable(type);
        }

        private bool IsTypeOfIEnumerable(Type type)
        {

            foreach (Type interfaceType in type.GetInterfaces())
            {

                if (interfaceType == typeof(IList))
                    return true;
            }

            return false;
        }

        private object ReadStream(Type type, Stream stream)
        {
            Type itemType;
            var typeIsArray = false;
            IList list;
            if (type.GetGenericArguments().Length > 0)
            {
                itemType = type.GetGenericArguments()[0];
                list = (IList)Activator.CreateInstance(type);
            }
            else
            {
                typeIsArray = true;
                itemType = type.GetElementType();

                var listType = typeof(List<>);
                var constructedListType = listType.MakeGenericType(itemType);

                list = (IList)Activator.CreateInstance(constructedListType);
            }

            var reader = new StreamReader(stream, Encoding.GetEncoding(_options.Encoding));

            bool skipFirstLine = _options.UseSingleLineHeaderInCsv;
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var values = line.Split(_options.CsvDelimiter.ToCharArray());
                if(skipFirstLine)
                {
                    skipFirstLine = false;
                }
                else
                {
                    var itemTypeInGeneric = list.GetType().GetTypeInfo().GenericTypeArguments[0];
                    var item = Activator.CreateInstance(itemTypeInGeneric);
                    var properties = item.GetType().GetProperties();
                    for (int i = 0;i<values.Length; i++)
                    {
                        properties[i].SetValue(item, Convert.ChangeType(values[i], properties[i].PropertyType), null);
                    }

                    list.Add(item);
                }

            }

            if(typeIsArray)
            {
                Array array = Array.CreateInstance(itemType, list.Count);

                for(int t = 0; t < list.Count; t++)
                {
                    array.SetValue(list[t], t);
                }
                return array;
            }
            
            return list;
        }
    }
}
```


This data can then be used to upload the csv data to the server which is then converted back to a C# object. I use fiddler, postman or curl can also be used, or any HTTP Client where you can set the header Content-Type.

```csharp

 http://localhost:10336/api/csvtest/import 

 User-Agent: Fiddler 
 Content-Type: text/csv 
 Host: localhost:10336 
=


 caller_id;recipient;call_date;duration;cost; reference;currency
 441215598896;447595657548;22-08-2016;'13:24:10;42;CDEB695CAC6C9E74744A472905001CBB8;GBP 


