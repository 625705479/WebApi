namespace WebApiProject1.Application.Test.Dtos
{
    public class TestTable
    {
        public string Name { get; set; }
        [SugarColumn(IsPrimaryKey = true)] // 标记为主键
        public int Id { get; set; }
        public int Age { get; set; }

    }
}