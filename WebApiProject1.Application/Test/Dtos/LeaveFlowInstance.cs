using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiProject1.Application.UntinesHelper;

namespace WebApiProject1.Application.Test.Dtos
{
    /// <summary>请假流程实例</summary>
    public class LeaveFlowInstance
    {
        /// <summary>流程实例ID</summary>
        public string InstanceId { get; set; }
        /// <summary>申请人ID</summary>
        public string ApplyUserId { get; set; }
        /// <summary>请假天数</summary>
        public int LeaveDays { get; set; }
        /// <summary>请假原因</summary>
        public string LeaveReason { get; set; }

        /// <summary>流程当前状态</summary>
        public LeaveFlowStatus CurrentStatus { get; set; }
        /// <summary>
        /// 流程状态中文文字显示
        /// </summary>
        public string CurrentStatusText { get; set; }


        public string? ManagerAuditComment { get; set; }
        public bool? ManagerAgree { get; set; }

        public string? DirectorAuditComment { get; set; }
       

        //新增总经理
        public string? GeneralManagerAuditComment { get; set; }
        public bool? GeneralManagerAgree { get; set; }

        /// <summary>经理是否同意</summary>
  
        /// <summary>经理审批意见</summary>
        public string ManagerComment { get; set; }
        /// <summary>经理审批时间</summary>
        public DateTime? ManagerAuditTime { get; set; }

        /// <summary>总监是否同意</summary>
        public bool? DirectorAgree { get; set; }
        /// <summary>总监审批意见</summary>
        public string DirectorComment { get; set; }
        /// <summary>总监审批时间</summary>
        public DateTime? DirectorAuditTime { get; set; }

        /// <summary>创建时间</summary>
        public DateTime CreateTime { get; set; }

        /// <summary>流程全部操作记录</summary>
        public List<LeaveFlowOperationRecord> OperationRecords { get; set; } = new List<LeaveFlowOperationRecord>();
    }


    /// <summary>请假单操作记录</summary>
    public class LeaveFlowOperationRecord
    {
        /// <summary>记录ID</summary>
        public string RecordId { get; set; }
        /// <summary>关联流程InstanceId</summary>
        public string InstanceId { get; set; }
        /// <summary>操作类型：发起申请/经理审批/总监审批/总经理审批</summary>
        public string OperationType { get; set; }
        /// <summary>操作人ID</summary>
        public string OperatorUserId { get; set; }
        /// <summary>操作时间</summary>
        public DateTime OperateTime { get; set; }
        /// <summary>审批意见/备注</summary>
        public string Comment { get; set; }
        /// <summary>审批是否同意，null=发起申请</summary>
        public bool? IsAgree { get; set; }
        /// <summary>操作前状态中文</summary>
        public string BeforeStatusText { get; set; }
        /// <summary>操作后状态中文</summary>
        public string AfterStatusText { get; set; }
    }

    /// <summary>请假流程状态</summary>
    public enum LeaveFlowStatus
    {

        /// <summary>已创建，等待经理审批</summary>
        [Description("已创建，等待经理审批")]
        Created = 0,

        /// <summary>经理审批中</summary>
        [Description("经理审批中")]
        ManagerAuditing = 1,

        /// <summary>总监审批中</summary>
        [Description("总监审批中")]
        DirectorAuditing = 2,

        /// <summary>总经理审批中</summary>
        [Description("总经理审批中")]
        GeneralManagerAuditing = 3,

        /// <summary>审批全部通过</summary>
        [Description("审批全部通过")]
        Approved = 4,

        /// <summary>被拒绝（经理/总监/总经理拒绝）</summary>
        [Description("被拒绝（经理/总监/总经理拒绝）")]
        Rejected = 5
    }
}
