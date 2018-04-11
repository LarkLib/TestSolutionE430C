using AutoMapper;
using AutoMapper.Configuration.Conventions;
using AutoMapper.Mappers;
using System;
using System.Linq;

namespace TestAutoMapperConsoleApplication
{
    class TestAutoMapper
    {
        public void Execute()
        {
            TestMehtodAutoMapper();
        }
        private void TestMehtodAutoMapper()
        {
            var source = new Source()
            {
                FirstName = "fff",
                LastName = "lll",
                EmployeeID = 100,
                LoginID = "domain\\admin",
                ManagerID = 10,
                Title = "Title001",
                HireDate = DateTime.Parse("2004-09-21")
            };
            {
                var config = new MapperConfiguration(cfg => cfg.CreateMap<Source, Destination>());
                var mapper = config.CreateMapper();
                var result = mapper.Map<Destination>(source);
            }
            {
                Mapper.Initialize(cfg => cfg.CreateMap<Source, Destination>());
                var result = Mapper.Map<Destination>(source);
            }
            {
                //Before and after map actions
                //Mapper.Initialize(cfg =>
                //{
                //    cfg.CreateMap<Source, Destination>()
                //      .BeforeMap((src, dest) => src.EmployeeID = src.EmployeeID * 20)
                //      .AfterMap((src, dest) => dest.FullName = "John Park");
                //});
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<string, int>().ConvertUsing((s) => Convert.ToInt32(s));//Custom type converters
                    cfg.CreateMap<string, DateTime>().ConvertUsing(new DateTimeTypeConverter());//Custom type converters
                    cfg.CreateMap<Source, Destination>()
                       .BeforeMap((src, dest) => src.EmployeeID = src.EmployeeID * 20)//Before  map actions
                       .AfterMap((src, dest) => dest.FullName = "John Park")//After map actions
                       .ForMember(dest => dest.EmployeeId, opt => opt.Condition(src => (src.EmployeeID >= 0)))
                       .ForMember(dest => dest.FullName, opt => opt.Condition(src => (!string.IsNullOrEmpty(src.FirstName) || !string.IsNullOrEmpty(src.LastName))))
                       .ForCtorParam("firstName", opt => opt.MapFrom(src => src.FirstName))
                       .ForCtorParam("lastName", opt => opt.MapFrom(src => src.LastName))
                       ;
                    //cfg.DisableConstructorMapping(); //You can also disable constructor mapping : 
                    cfg.AddConditionalObjectMapper().Where((s, d) => s.Name == d.Name + "Dto");//Conditional Object Mapper
                    cfg.AddConditionalObjectMapper().Where((s, d) => s.Name == d.Name + "Time");//Conditional Object Mapper
                    cfg.AddMemberConfiguration()//Member Configuration
                    .AddMember<NameSplitMember>()//gets you default naming convention functionality.
                    //.AddMember<ReplaceName>(n => n.AddReplace("ID", "id"))
                    ;
                });
                var mapper = config.CreateMapper();
                var result = mapper.Map<Destination>(source);
                int i = 10;
                Mapper.Initialize(cfg => cfg.CreateMap<Source, Destination>());
                Mapper.Map<Source, Destination>(source, opt =>
                  {
                      opt.BeforeMap((s, d) => s.EmployeeID = s.EmployeeID * i);
                      opt.AfterMap((s, d) => d.FullName = d.FullName = "Hanks Tom");
                  });
            }
        }
    }
    public class DateTimeTypeConverter : ITypeConverter<string, DateTime>
    {
        public DateTime Convert(string source, DateTime destination, ResolutionContext context)
        {
            return System.Convert.ToDateTime(source);
        }
    }

    public class Source
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int EmployeeID { get; set; }
        public string LoginID { get; set; }
        public Nullable<int> ManagerID { get; set; }
        public string Title { get; set; }
        public DateTime HireDate { get; set; }
        [MapTo("Level")]
        public int JobGread { get; set; }
    }

    public class Destination
    {
        public Destination(string firstName, string lastName)
        {
            this.FullName = $"{firstName} {lastName}";
        }
        public string FullName { get; set; }
        public int EmployeeId { get; set; }
        public string emailAddress { get; set; }

        public Nullable<int> ManagerId { get; set; }
        public string TitleDto { get; set; }
        public string Level { get; set; }
        public DateTime HireDateTime { get; set; }
        public DateTime Birthday { get; set; }
    }

}
