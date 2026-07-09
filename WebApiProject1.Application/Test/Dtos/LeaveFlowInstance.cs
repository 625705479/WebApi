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
