using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.AR
{
    public enum AccessRequestStatusEnum : int
    {

        [Display(Name = "Initiated")]
        Initiated = 1,

        [Display(Name = "Rejected")]
        Rejected = 2,

        [Display(Name = "Pending")]
        Pending = 10,

        //below two enum values is for supporting multiple clarification cycles between HR and requester in the
        //remote access module

        [Display(Name = "Clarification Requested")]
        ClarificationRequested = 11,

        [Display(Name = "Clarification Provided")]
        ClarificationProvided = 12,

        [Display(Name = "Completed")]
        Completed = 25,

        // marking remote access request as pending revoke confirmation from user
        [Display(Name = "Revoke Confirmation Pending")]
        RevokeConfirmationPending = 26,

        // marking remote access request as confirmation received from user to continue keeping access
        [Display(Name = "Revoke Confirmed Continue")]
        RevokeConfirmedContinue = 27,

        // marking remote access request as to be revoked if user not respond in two weeks time to a revoke confirmation
        [Display(Name = "To be Revoked")]
        ToBeRevoked = 28,

        // marking remote access request as "revoked" if implementer revoked it
        // during 2 weeks time period from it been marked as "to be revoked"
        [Display(Name = "Revoked")]
        Revoked = 29,

        // for marking remote access request as expired
        [Display(Name = "Expired")] 
        Expired = 35,



    }
}
