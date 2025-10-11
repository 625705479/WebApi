using Furion.DataValidation;
using Microsoft.Extensions.Options;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using StackExchange.Profiling.Internal;
using System;
using System.Collections;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using WebApiProject1.Application.Test.Dtos;
using WebApiProject1.Application.UntinesHelper;
using WebApiProject1.Core;

namespace WebApiProject1.Application.Test.Services
{
    public class TestService : ITestService, ITransient
    {
        #region 原有辅助方法（保留并适配整合逻辑）
        /// <summary>
        /// 从Excel生成XML配置文件的工具类
        /// </summary>
        public class ExcelToXmlGenerator
        {
            // 常量定义 - 集中管理固定值，便于维护
            private const int InitialOrdinal = 2; // 起始序号（参考示例XML的ordinal规则）
            private const string MonitorSuffix = "Monitor"; // 监控名称后缀

            /// <summary>
            /// 从Excel生成XML的主方法
            /// </summary>
            /// <param name="excelPath">Excel文件路径</param>
            /// <param name="xmlPath">源XML文件路径</param>
            /// <param name="outputXmlPath">输出XML文件路径，默认覆盖源文件</param>
            public static void GenerateXmlFromExcel(string excelPath, string xmlPath, string outputXmlPath = null)
            {
                try
                {
                    // 处理输出路径，默认覆盖原文件
                    outputXmlPath ??= xmlPath;

                    Console.WriteLine("开始处理...");
                    Console.WriteLine($"Excel路径: {excelPath}");
                    Console.WriteLine($"XML源路径: {xmlPath}");
                    Console.WriteLine($"XML输出路径: {outputXmlPath}");

                    // 1. 读取Excel数据并验证
                    var alarmList = ReadExcelData(excelPath);
                    ValidateAlarmList(alarmList);
                    Console.WriteLine($"成功读取 {alarmList.Count} 条告警数据");

                    // 2. 加载并验证XML文档
                    var xmlDoc = LoadAndValidateXmlDocument(xmlPath);
                    Console.WriteLine("成功加载XML文档");

                    // 3. 定位目标节点
                    var (alertConfigurationsNode, propertyDefinitionsNode) = FindTargetNodes(xmlDoc);
                    Console.WriteLine("成功定位目标节点");

                    // 4. 清空原有节点（如需保留历史数据，可修改此逻辑）
                    ClearExistingNodes(alertConfigurationsNode, propertyDefinitionsNode);
                    Console.WriteLine("已清空原有节点数据");

                    // 5. 生成并添加新节点
                    GenerateAndAddNewNodes(alarmList, alertConfigurationsNode, propertyDefinitionsNode);
                    Console.WriteLine("已生成并添加新节点");

                    // 6. 保存修改后的XML
                    SaveXmlDocument(xmlDoc, outputXmlPath);
                    Console.WriteLine($"XML更新成功，路径：{outputXmlPath}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"操作失败：{ex.Message}");
                    // 如需调试可添加堆栈信息
                    // Console.WriteLine($"详细错误：{ex.StackTrace}");
                }
            }

            /// <summary>
            /// 读取Excel数据
            /// </summary>
            private static List<AlarmInfo> ReadExcelData(string excelPath)
            {
                var result = new List<AlarmInfo>();

                if (!File.Exists(excelPath))
                    throw new FileNotFoundException("Excel文件不存在", excelPath);

                // 根据文件后缀创建对应的Workbook
                IWorkbook workbook;
                using (var stream = new FileStream(excelPath, FileMode.Open, FileAccess.Read))
                {
                    if (excelPath.EndsWith(".xlsx"))
                        workbook = new XSSFWorkbook(stream); // .xlsx格式
                    else if (excelPath.EndsWith(".xls"))
                        workbook = new HSSFWorkbook(stream); // .xls格式
                    else
                        throw new NotSupportedException("不支持的Excel格式（仅支持.xls和.xlsx）");
                }

                // 读取第一个工作表
                ISheet sheet = workbook.GetSheetAt(0);
                if (sheet == null)
                    throw new Exception("Excel中无工作表");

                // 查找表头列索引（基础列：告警定义、告警描述、字段类型）
                int nameColIndex = -1;
                int descColIndex = -1;
                int typeColIndex = -1;

                IRow headerRow = sheet.GetRow(0); // 表头行（第1行，索引0）
                if (headerRow == null)
                    throw new Exception("Excel中未找到表头行");

                for (int col = 0; col < headerRow.LastCellNum; col++)
                {
                    ICell cell = headerRow.GetCell(col);
                    if (cell == null) continue;

                    string cellValue = cell.StringCellValue.Trim();
                    if (cellValue == "告警定义")
                        nameColIndex = col;
                    else if (cellValue == "告警描述")
                        descColIndex = col;
                    else if (cellValue == "字段类型")
                        typeColIndex = col;
                }

                // 验证表头是否完整
                if (nameColIndex == -1 || descColIndex == -1 || typeColIndex == -1)
                    throw new Exception("Excel表头必须包含：告警定义、告警描述、字段类型");

                // 读取数据行（从第2行开始，索引1）
                for (int rowIndex = 1; rowIndex <= sheet.LastRowNum; rowIndex++)
                {
                    IRow dataRow = sheet.GetRow(rowIndex);
                    if (dataRow == null) continue;

                    // 读取“告警定义”（必填）
                    string name = GetCellValue(dataRow.GetCell(nameColIndex));
                    if (string.IsNullOrWhiteSpace(name))
                        continue; // 跳过空行

                    // 读取“告警描述”和“字段类型”
                    string description = GetCellValue(dataRow.GetCell(descColIndex));
                    string baseType = GetCellValue(dataRow.GetCell(typeColIndex));

                    // 字段类型为空时默认STRING
                    if (string.IsNullOrWhiteSpace(baseType))
                        baseType = "STRING";

                    result.Add(new AlarmInfo
                    {
                        Name = name,
                        Description = description,
                        BaseType = baseType
                        // 如需从Excel读取AlertType和Priority，可在此扩展
                    });
                }

                return result;
            }
            /// <summary>
            /// 获取单元格的值（兼容不同数据类型）
            /// </summary>
            private static string GetCellValue(ICell cell)
            {
                if (cell == null)
                    return string.Empty;

                switch (cell.CellType)
                {
                    case CellType.String:
                        return cell.StringCellValue.Trim();
                    case CellType.Numeric:
                        if (DateUtil.IsCellDateFormatted(cell))
                            return cell.DateCellValue.ToString();
                        else
                            return cell.NumericCellValue.ToString();
                    case CellType.Boolean:
                        return cell.BooleanCellValue.ToString();
                    default:
                        return string.Empty;
                }
            }


            /// <summary>
            /// 验证告警列表数据
            /// </summary>
            private static void ValidateAlarmList(List<AlarmInfo> alarmList)
            {
                if (alarmList == null || alarmList.Count == 0)
                    throw new Exception("Excel中未读取到有效数据");

                // 检查必填字段
                var invalidItems = alarmList
                    .Where(a => string.IsNullOrWhiteSpace(a.Name) ||
                               string.IsNullOrWhiteSpace(a.BaseType) ||
                               string.IsNullOrWhiteSpace(a.AlertType))
                    .ToList();

                if (invalidItems.Any())
                    throw new Exception($"发现{invalidItems.Count}条无效数据：告警名称、基础类型和告警类型不能为空");
            }

            /// <summary>
            /// 加载并验证XML文档
            /// </summary>
            private static XDocument LoadAndValidateXmlDocument(string xmlPath)
            {
                try
                {
                    if (!File.Exists(xmlPath))
                        throw new FileNotFoundException("XML文件不存在", xmlPath);

                    return XDocument.Load(xmlPath);
                }
                catch (Exception ex)
                {
                    throw new Exception($"加载XML文档失败：{ex.Message}", ex);
                }
            }

            /// <summary>
            /// 查找目标节点并验证
            /// </summary>
            private static (XElement alertConfigs, XElement propertyDefinitions) FindTargetNodes(XDocument xmlDoc)
            {
                var alertConfigs = xmlDoc.Descendants("AlertConfigurations").FirstOrDefault();
                var propertyDefinitions = xmlDoc.Descendants("ThingShape")
                                                .Elements("PropertyDefinitions")
                                                .FirstOrDefault();

                if (alertConfigs == null)
                    throw new Exception("XML中未找到AlertConfigurations节点");

                if (propertyDefinitions == null)
                    throw new Exception("XML中未找到PropertyDefinitions节点");

                return (alertConfigs, propertyDefinitions);
            }

            /// <summary>
            /// 清空现有节点
            /// </summary>
            private static void ClearExistingNodes(XElement alertConfigurations, XElement propertyDefinitions)
            {
                alertConfigurations.RemoveAll();
                propertyDefinitions.RemoveAll();
            }

            /// <summary>
            /// 生成并添加新节点
            /// </summary>
            private static void GenerateAndAddNewNodes(List<AlarmInfo> alarmList,
                                              XElement alertConfigurationsNode,
                                              XElement propertyDefinitionsNode)
            {
                int ordinal = InitialOrdinal;

                foreach (var alarm in alarmList)
                {
                    // 添加AlertDefinitions节点
                    var alertDefinitions = CreateAlertDefinitions(alarm);
                    alertConfigurationsNode.Add(alertDefinitions);

                    // 添加PropertyDefinition节点
                    var propertyDefinition = CreatePropertyDefinition(alarm, ordinal);
                    propertyDefinitionsNode.Add(propertyDefinition);

                    ordinal++;
                }
            }

            /// <summary>
            /// 创建AlertDefinitions节点
            /// </summary>
            private static XElement CreateAlertDefinitions(AlarmInfo alarm)
            {
                var alertDefinitions = new XElement("AlertDefinitions",
                    new XAttribute("name", alarm.Name)
                );

                // 根据基础类型创建不同的AlertDefinition
                if (IsBooleanType(alarm.BaseType))
                {
                    alertDefinitions.Add(CreateBooleanAlertDefinition(alarm));
                }
                else
                {
                    alertDefinitions.Add(CreateNonBooleanAlertDefinition(alarm));
                }

                return alertDefinitions;
            }

            /// <summary>
            /// 创建布尔类型的AlertDefinition
            /// </summary>
            private static XElement CreateBooleanAlertDefinition(AlarmInfo alarm)
            {
                return new XElement("AlertDefinition",
                    new XAttribute("alertType", alarm.AlertType),
                    new XAttribute("description", alarm.Description),
                    new XAttribute("enabled", "true"),
                    new XAttribute("name", $"{alarm.Name}{MonitorSuffix}"),
                    new XAttribute("priority", alarm.Priority),
                    new XAttribute("propertyBaseType", alarm.BaseType),
                    // AlertAttributes子节点
                    CreateAlertAttributes(alarm.BaseType, "true", false)
                );
            }

            /// <summary>
            /// 创建非布尔类型的AlertDefinition
            /// </summary>
            private static XElement CreateNonBooleanAlertDefinition(AlarmInfo alarm)
            {
                (string valueContent, bool isCData) = GetValueContentByBaseType(alarm.BaseType);

                return new XElement("AlertDefinition",
                    new XAttribute("alertType", alarm.AlertType),
                    new XAttribute("description", alarm.Description),
                    new XAttribute("enabled", "true"),
                    new XAttribute("name", $"{alarm.Name}{MonitorSuffix}"),
                    new XAttribute("priority", alarm.Priority),
                    new XAttribute("propertyBaseType", alarm.BaseType),
                    CreateAlertAttributes(alarm.BaseType, valueContent, isCData)
                );
            }

            /// <summary>
            /// 创建AlertAttributes节点
            /// </summary>
            private static XElement CreateAlertAttributes(string baseType, string valueContent, bool isCData)
            {
                return new XElement("AlertAttributes",
                    new XElement("DataShape",
                        new XElement("FieldDefinitions",
                            new XElement("FieldDefinition",
                                GetFieldDefinitionAttributes(baseType),
                                new XAttribute("description", "value"),
                                new XAttribute("name", "value"),
                                new XAttribute("ordinal", "0")
                            )
                        )
                    ),
                    new XElement("Rows",
                        new XElement("Row",
                            CreateValueElement(valueContent, isCData)
                        )
                    )
                );
            }

            /// <summary>
            /// 创建PropertyDefinition节点
            /// </summary>
            private static XElement CreatePropertyDefinition(AlarmInfo alarm, int ordinal)
            {
                return new XElement("PropertyDefinition",
                    new XAttribute("aspect.cacheTime", "0.0"),
                    new XAttribute("aspect.dataChangeType", "VALUE"),
                    new XAttribute("aspect.isPersistent", "true"),
                    new XAttribute("baseType", alarm.BaseType),
                    new XAttribute("category", ""),
                    new XAttribute("description", alarm.Description),
                    new XAttribute("isLocalOnly", "false"),
                    new XAttribute("name", alarm.Name),
                    new XAttribute("ordinal", ordinal.ToString())
                );
            }

            /// <summary>
            /// 保存XML文档
            /// </summary>
            private static void SaveXmlDocument(XDocument xmlDoc, string outputPath)
            {
                try
                {
                    // 确保输出目录存在
                    var outputDir = Path.GetDirectoryName(outputPath);
                    if (!string.IsNullOrEmpty(outputDir) && !Directory.Exists(outputDir))
                    {
                        Directory.CreateDirectory(outputDir);
                        Console.WriteLine($"已创建输出目录: {outputDir}");
                    }

                    xmlDoc.Save(outputPath);
                }
                catch (Exception ex)
                {
                    throw new Exception($"保存XML文件失败：{ex.Message}", ex);
                }
            }

            /// <summary>
            /// 判断是否为布尔类型
            /// </summary>
            private static bool IsBooleanType(string baseType)
            {
                return string.Equals(baseType, "BOOLEAN", StringComparison.OrdinalIgnoreCase);
            }

            /// <summary>
            /// 根据基础类型获取值内容
            /// </summary>
            private static (string value, bool isCData) GetValueContentByBaseType(string baseType)
            {
                return baseType.ToUpperInvariant() switch
                {
                    "STRING" => (" 1 ", true),
                    "NUMBER" => ("1.0", false),
                    "DATETIME" => ("2025-09-17T00:00:00.000+08:00", false),
                    "LONG" => ("1", false),
                    _ => throw new NotSupportedException($"不支持的基础类型: {baseType}")
                };
            }

            /// <summary>
            /// 获取字段定义的属性集合
            /// </summary>
            private static IEnumerable<XAttribute> GetFieldDefinitionAttributes(string baseType)
            {
                var attributes = new List<XAttribute>
        {
            new XAttribute("aspect.friendlyName", "Value"),
            new XAttribute("aspect.isRequired", "true"),
            new XAttribute("baseType", baseType)
        };

                // 布尔类型需要默认值属性
                if (IsBooleanType(baseType))
                {
                    attributes.Add(new XAttribute("aspect.defaultValue", "false"));
                }

                return attributes;
            }

            /// <summary>
            /// 创建值元素（根据需要使用CDATA）
            /// </summary>
            private static XElement CreateValueElement(string content, bool isCData)
            {
                return isCData
                    ? new XElement("value", new XCData(content))
                    : new XElement("value", content);
            }

            private class AlarmInfo
            {
                public string Name { get; set; }           // 对应Excel“告警定义”列（XML的name属性）
                public string Description { get; set; }    // 对应Excel“告警描述”列（XML的description属性）
                public string BaseType { get; set; }       // 对应Excel“字段类型”列（XML的baseType属性）
                public string AlertType { get; set; } = "EqualTo"; // 告警类型（默认EqualTo，可从Excel扩展）
                public int Priority { get; set; } = 1;     // 告警优先级（默认1，可从Excel扩展）

            }
        }
        /// <summary>
        /// 增强版替换XML中所有指定的值（支持多组替换对）
        /// </summary>
        /// 

        private static void ReplaceXmlValues(XDocument doc, string oldVal1, string newVal1, string oldVal2, string newVal2, string oldVal3, string newVal3)
        {
            // 替换所有元素的文本内容
            foreach (var element in doc.Descendants())
            {
                if (element.Value.Contains(oldVal1)) element.Value = element.Value.Replace(oldVal1, newVal1);
                if (element.Value.Contains(oldVal2)) element.Value = element.Value.Replace(oldVal2, newVal2);
                if (element.Value.Contains(oldVal3)) element.Value = element.Value.Replace(oldVal3, newVal3);

                // 替换所有属性值
                foreach (var attribute in element.Attributes())
                {
                    if (attribute.Value.Contains(oldVal1)) attribute.Value = attribute.Value.Replace(oldVal1, newVal1);
                    if (attribute.Value.Contains(oldVal2)) attribute.Value = attribute.Value.Replace(oldVal2, newVal2);
                    if (attribute.Value.Contains(oldVal3)) attribute.Value = attribute.Value.Replace(oldVal3, newVal3);
                }
            }
        }

        /// <summary>
        /// 从Excel读取指定列的name值（兼容.xls/.xlsx）
        /// </summary>
        private static List<string> ReadNamesFromExcel(string filePath, int columnIndex, int startRow)
        {
            var names = new List<string>();
            IWorkbook workbook = null;

            try
            {
                using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    // 根据文件格式创建Workbook
                    if (filePath.EndsWith(".xls", StringComparison.OrdinalIgnoreCase))
                        workbook = new HSSFWorkbook(stream);
                    else if (filePath.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
                        workbook = new XSSFWorkbook(stream);
                    else
                        throw new Exception("不支持的Excel格式（仅支持.xls/.xlsx）");
                }

                // 读取第一个工作表
                ISheet sheet = workbook.GetSheetAt(0);
                if (sheet == null) return names;

                // 遍历行（从startRow开始，跳过表头）
                int lastRow = sheet.LastRowNum;
                for (int rowNum = startRow; rowNum <= lastRow; rowNum++)
                {
                    IRow row = sheet.GetRow(rowNum);
                    if (row == null) continue;

                    // 读取指定列的单元格值
                    ICell cell = row.GetCell(columnIndex);
                    string cellValue = GetCellValue(cell).Trim();
                    if (!string.IsNullOrEmpty(cellValue))
                        names.Add(cellValue);
                }
            }
            finally
            {
                workbook?.Close(); // 释放资源
            }
            return names;
        }

        /// <summary>
        /// 获取Excel单元格的文本值（兼容不同数据类型）
        /// </summary>
        private static string GetCellValue(ICell cell)
        {
            if (cell == null) return string.Empty;

            return cell.CellType switch
            {
                CellType.String => cell.StringCellValue,
                CellType.Numeric => DateUtil.IsCellDateFormatted(cell) ? cell.DateCellValue.ToString() : cell.NumericCellValue.ToString(),
                CellType.Boolean => cell.BooleanCellValue.ToString(),
                CellType.Formula => cell.CellFormula, // 公式单元格暂取公式文本（可根据需求改为计算结果）
                _ => string.Empty
            };
        }

        /// <summary>
        /// 从Thing.xml路径提取sourceThingName前缀（原逻辑保留）
        /// </summary>
        private static string ExtractTargetPart(string filePath)
        {
            string fileName = Path.GetFileName(filePath);
            string fileNameWithoutExt = Path.GetFileNameWithoutExtension(fileName);
            string afterThings = fileNameWithoutExt.Replace("Things_", string.Empty);
            string beforeThing = afterThings.Replace(".Thing", string.Empty);
            return beforeThing + ".";
        }
        #endregion

        /// <summary>
        /// 创建或者修改xml文件
        /// </summary>
        /// <param name="ThingxmlPath">Thingxml路径</param>
        /// <param name="RemoteThingPath">RemoteThing路径</param>
        /// <param name="ThingTemplatespPath">ThingTemplates路径</param>
        /// <param name="ExcelPath">ExcelPath路径</param>
        /// <param name="originalNumber">原始目标数字</param>
        /// <param name="RepaceNumber">替换的数字</param>
        /// <returns></returns>

        public bool CreateOrSaveFile(string ThingxmlPath, string RemoteThingPath, string ThingTemplatespPath, string ExcelPath, string originalNumber, string RepaceNumber)
        {
            string thingOutputDir = Path.Combine(Path.GetDirectoryName(ThingxmlPath), "create_thing");
            string remoteThingOutputDir = Path.Combine(Path.GetDirectoryName(RemoteThingPath), "create_remote_thing");
            Console.WriteLine("=== 开始处理ThingTemplates.xml ===");
            ExcelToXmlGenerator.GenerateXmlFromExcel(ExcelPath, ThingTemplatespPath);
            Console.WriteLine("=== 处理结束ThingTemplates.xml ===");
            string[] targetNumbers = RepaceNumber.Split(',', StringSplitOptions.RemoveEmptyEntries);
            try
            {
                // -------------------------- 步骤1：批量生成Thing.xml（原CreateThingXml逻辑） --------------------------
                Console.WriteLine("=== 开始生成Thing.xml ===");
                // 加载原始Thing.xml
                if (!File.Exists(ThingxmlPath))
                {
                    Console.WriteLine($"错误：原始Thing.xml不存在 - {ThingxmlPath}");
                    return false;
                }
                XDocument originalThingDoc = XDocument.Load(ThingxmlPath);

                // 提取模块名（如从路径中获取"TDTLAMINATEDREFLUXLINEM1001"）
                string thingModuleName = ThingxmlPath.Split('.')[2];
                string thingDevcidata = Regex.Replace(thingModuleName, @"\d", string.Empty); // 移除数字，保留前缀

                // 确保输出目录存在
                if (!Directory.Exists(thingOutputDir))
                {
                    Directory.CreateDirectory(thingOutputDir);
                    Console.WriteLine($"已创建Thing输出目录：{thingOutputDir}");
                }

                // 遍历目标编号生成文件
                foreach (string targetNumber in targetNumbers)
                {
                    XDocument newThingDoc = new XDocument(originalThingDoc); // 复制文档避免污染原文件
                    var thingNode = newThingDoc.Descendants("Thing").FirstOrDefault();

                    // 1.1 替换Thing节点的name属性
                    if (thingNode != null && thingNode.Attribute("name") != null)
                    {
                        thingNode.Attribute("name").Value = thingNode.Attribute("name").Value.Replace(originalNumber, targetNumber);
                    }

                    // 1.2 替换Thing节点的description属性
                    if (thingNode != null && thingNode.Attribute("description") != null)
                    {
                        thingNode.Attribute("description").Value = thingNode.Attribute("description").Value.Replace(originalNumber, targetNumber);
                    }

                    // 1.3 替换所有PropertyBinding的sourceName和sourceThingName
                    foreach (var propBinding in newThingDoc.Descendants("PropertyBinding"))
                    {
                        if (propBinding.Attribute("sourceName") != null)
                            propBinding.Attribute("sourceName").Value = propBinding.Attribute("sourceName").Value.Replace(originalNumber, targetNumber);
                        if (propBinding.Attribute("sourceThingName") != null)
                            propBinding.Attribute("sourceThingName").Value = propBinding.Attribute("sourceThingName").Value.Replace(originalNumber, targetNumber);
                    }

                    // 1.4 替换equipmentNo的Value
                    var equipmentNoValue = newThingDoc.Descendants("ThingProperties")
                                                    .Elements("equipmentNo")
                                                    .Elements("Value")
                                                    .FirstOrDefault();
                    if (equipmentNoValue != null)
                    {
                        equipmentNoValue.Value = equipmentNoValue.Value.Replace(originalNumber, targetNumber);
                    }

                    // 1.5 替换ConfigurationChange的changeReason
                    foreach (var configChange in newThingDoc.Descendants("ConfigurationChange"))
                    {
                        if (configChange.Attribute("changeReason") != null)
                            configChange.Attribute("changeReason").Value = configChange.Attribute("changeReason").Value.Replace(originalNumber, targetNumber);
                    }

                    // 生成输出路径并保存
                    string thingOutputFileName = $"Things_TS.Module.{thingDevcidata}{targetNumber}.Alarm.Thing.xml";
                    string thingOutputPath = Path.Combine(thingOutputDir, thingOutputFileName);

                    var xmlSettings = new XmlWriterSettings
                    {
                        Encoding = UTF8Encoding.UTF8,
                        Indent = true,
                        IndentChars = "  ",
                        OmitXmlDeclaration = false,
                        NewLineHandling = NewLineHandling.None
                    };

                    using (var writer = XmlWriter.Create(thingOutputPath, xmlSettings))
                    {
                        newThingDoc.Save(writer);
                    }
                    Console.WriteLine($"已生成Thing文件：{thingOutputPath}");
                }
                Console.WriteLine("=== Thing.xml生成完成 ===");


                // -------------------------- 步骤2：批量生成RemoteThing.xml（原CreateRemoteThingXml逻辑） --------------------------
                Console.WriteLine("\n=== 开始生成RemoteThing.xml ===");
                // 检查原始RemoteThing.xml是否存在
                if (!File.Exists(RemoteThingPath))
                {
                    Console.WriteLine($"错误：原始RemoteThing.xml不存在 - {RemoteThingPath}");
                    return false;
                }

                // 确保输出目录存在
                if (!Directory.Exists(remoteThingOutputDir))
                {
                    Directory.CreateDirectory(remoteThingOutputDir);
                    Console.WriteLine($"已创建RemoteThing输出目录：{remoteThingOutputDir}");
                }

                // 加载原始RemoteThing.xml
                XDocument originalRemoteDoc = XDocument.Load(RemoteThingPath);
                string remoteModuleName = RemoteThingPath.Split('.')[2];
                string remoteDevcidata = Regex.Replace(remoteModuleName, @"\d", string.Empty);

                // 遍历目标编号生成文件
                foreach (string targetNumber in targetNumbers)
                {
                    XDocument newRemoteDoc = new XDocument(originalRemoteDoc);

                    // 替换所有编号相关内容（调用辅助方法）
                    ReplaceXmlValues(
                        newRemoteDoc,
                        originalNumber,          // 原始基础编号（如1001）
                        targetNumber,            // 目标基础编号（如1002）
                        $"{originalNumber}",    // 原始带标识编号（原逻辑保留，可根据实际调整）
                        $"{targetNumber}",      // 目标带标识编号
                        $"{originalNumber}_",   // 原始下划线编号（如1001_）
                        $"{targetNumber}_"      // 目标下划线编号
                    );

                    // 生成输出路径并保存
                    string remoteOutputFileName = $"Things_TS.Module.{remoteDevcidata}{targetNumber}.Alarm.RemoteThing.xml";
                    string remoteOutputPath = Path.Combine(remoteThingOutputDir, remoteOutputFileName);

                    var remoteXmlSettings = new XmlWriterSettings
                    {
                        Encoding = UTF8Encoding.UTF8,
                        Indent = true,
                        IndentChars = "  ",
                        OmitXmlDeclaration = false,
                        NewLineHandling = NewLineHandling.None
                    };

                    using (var writer = XmlWriter.Create(remoteOutputPath, remoteXmlSettings))
                    {
                        newRemoteDoc.Save(writer);
                    }
                    Console.WriteLine($"已生成RemoteThing文件：{remoteOutputPath}");
                }
                Console.WriteLine("=== RemoteThing.xml生成完成 ===");


                // -------------------------- 步骤3：批量处理PropertyBinding（原PropertyBindServices逻辑） --------------------------
                Console.WriteLine("\n=== 开始处理PropertyBinding ===");
                // 检查Excel文件是否存在
                if (!File.Exists(ExcelPath))
                {
                    Console.WriteLine($"错误：Excel文件不存在 - {ExcelPath}");
                    return false;
                }

                // 获取所有生成的Thing.xml文件（匹配命名规则）
                if (!Directory.Exists(thingOutputDir))
                {
                    Console.WriteLine($"错误：Thing输出目录不存在 - {thingOutputDir}");
                    return false;
                }
                string[] generatedThingFiles = Directory.GetFiles(thingOutputDir, "Things_TS.Module.*.Alarm.Thing.xml", SearchOption.TopDirectoryOnly);
                if (generatedThingFiles.Length == 0)
                {
                    Console.WriteLine($"警告：未找到生成的Thing.xml文件，跳过PropertyBinding处理");
                    return true; // 无文件可处理，视为成功（或根据需求改为false）
                }

                // 遍历每个生成的Thing.xml，添加PropertyBinding
                foreach (string thingFilePath in generatedThingFiles)
                {
                    Console.WriteLine($"\n处理文件：{thingFilePath}");
                    XDocument thingDoc = XDocument.Load(thingFilePath);

                    // 3.1 定位PropertyBindings节点
                    var propBindingsNode = thingDoc.Descendants("PropertyBindings").FirstOrDefault();
                    if (propBindingsNode == null)
                    {
                        Console.WriteLine($"警告：未找到PropertyBindings节点，跳过该文件");
                        continue;
                    }

                    // 3.2 从Excel读取name列表（列索引0=A列，行索引1=跳过表头）
                    List<string> excelNames = ReadNamesFromExcel(ExcelPath, 0, startRow: 1);
                    if (excelNames.Count == 0)
                    {
                        Console.WriteLine($"警告：未从Excel读取到name数据，跳过该文件");
                        continue;
                    }

                    // 3.3 提取当前Thing文件的目标编号（用于构造sourceName）
                    string fileName = Path.GetFileNameWithoutExtension(thingFilePath);
                    string devcidataWithNum = fileName.Split('.')[2]; // 格式：{devcidata}{targetNumber}
                    Match numMatch = Regex.Match(devcidataWithNum, @"\d+"); // 匹配数字部分
                    if (!numMatch.Success)
                    {
                        Console.WriteLine($"警告：无法提取目标编号，跳过该文件");
                        continue;
                    }
                    string currentTargetNum = numMatch.Value;

                    // 3.4 批量添加PropertyBinding（去重）
                    foreach (string name in excelNames)
                    {
                        // 去重检查
                        if (propBindingsNode.Elements("PropertyBinding").Any(b => b.Attribute("name")?.Value == name))
                        {
                            Console.WriteLine($"跳过重复绑定：name={name}");
                            continue;
                        }

                        // 构造sourceName和sourceThingName（动态适配编号）
                        string targetPart = ExtractTargetPart(thingFilePath);
                        string sourceName = $"TDT_LAMINATED_REFLUX_LINE_M{currentTargetNum}_M{currentTargetNum}_PLC_{name}"; // 原格式动态化
                        string sourceThingName = $"{targetPart}RemoteThing";

                        // 创建并添加节点
                        var bindingNode = new XElement("PropertyBinding",
                            new XAttribute("name", name),
                            new XAttribute("sourceName", sourceName),
                            new XAttribute("sourceThingName", sourceThingName)
                        );
                        propBindingsNode.Add(bindingNode);
                        Console.WriteLine($"添加绑定成功：name={name}");
                    }

                    // 3.5 保存修改后的Thing.xml
                    thingDoc.Save(thingFilePath);
                    Console.WriteLine($"文件保存成功：{thingFilePath}");
                }
                Console.WriteLine("=== PropertyBinding处理完成 ===");


                // 所有步骤执行成功
                Console.WriteLine("\n✅ 全部文件创建与处理完成！");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ 处理失败：{ex.Message}");
                return false;
            }
        }

        public string GetString()
        {
            return "Hello World";
        }
        /// <summary>
        /// 获取所有挡位信息
        /// </summary>
        /// <param name="gradingQuery"></param>
        /// <returns></returns>
        ResultData<object> ITestService.GetAllGradingDetailsAsync(GradingQueryDetail gradingQuery)
        {
            ResultData<object> resultData = new();
            var db = DbContext.Instance.GetConnection("PostgreSQLDB");
            int totalCount = 0;
            try
            {
                // 直接获取查询结果并显式指定类型，避免后续多次类型转换
                var queryResult = db.Queryable<GradingDetail>()
                                    .AS("grading_detail")
                                    .WhereIF(gradingQuery.grading_position.HasValue(), x => x.grading_position == gradingQuery.grading_position)
                                    .WhereIF(gradingQuery.item.HasValue(), x => x.item.Contains(gradingQuery.item))
                                    .ToPageList(gradingQuery.Pagenumber, gradingQuery.PageSize,ref totalCount);
                _ = new { TC = resultData.TotalCount = totalCount, PS = resultData.PageSize = gradingQuery.PageSize, PN = resultData.Pagenumber = gradingQuery.Pagenumber,Data = resultData.Data = queryResult };
                if (queryResult.Count == 0)
                {
                    Untines.SetError(resultData, EnumExtensions.MyErrorEnum.QueryError);
                }
                return resultData;
            }
            catch
            {
                Untines.SetError(resultData, EnumExtensions.MyErrorEnum.QueryError);
                return resultData;
            }
        }
        /// <summary>
        /// 查询单条数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResultData<object> ITestService.GetGradingDetailByIdAsync(int id)
        {
            var db = DbContext.Instance.GetConnection("PostgreSQLDB");
            ResultData<object> resultData = new();
            // 按ID查询单条记录
            resultData.Data = db.Queryable<GradingDetail>()
                           .AS("grading_detail", "o")
                           .Where(g => g.id == id)
                           .First();
            //判断dataList是否有数据
            if (resultData.Data == null)
            {
                Untines.SetError(resultData, EnumExtensions.MyErrorEnum.QueryError);
            }
            return resultData;
        }
        /// <summary>
        /// 添加或者修改
        /// </summary>
        /// <param name="test"></param>
        /// <returns></returns>
        public  ResultData<object> InsertOrUpdateDataAsync(TestTable test)
        {
            var db = DbContext.Instance.GetConnection("SqliteDB");
            ResultData<object> resultData = new();
            object res = null;
            var x = db.Storageable<TestTable>(new TestTable { Id = test.Id, Name = test.Name ,Age = test.Age }).As("test").ToStorage();
            res = x.AsInsertable.ExecuteCommand();
            res = x.AsUpdateable.ExecuteCommand();
            return resultData;
        }
        /// <summary>
        /// 根据id删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResultData<object> DeleteDataAsync(int id)
        {
            var db = DbContext.Instance.GetConnection("SqliteDB");
            ResultData<object> resultData = new();
            int res =  db.Deleteable<TestTable>()
                               .AS("test") 
                               .Where(t => t.Id == id) 
                               .ExecuteCommand();
            if(res==0)
            {
                Untines.SetError(resultData, EnumExtensions.MyErrorEnum.FailedToDeleteData);
                return resultData;
            }
                return resultData;
        }
    }
}
