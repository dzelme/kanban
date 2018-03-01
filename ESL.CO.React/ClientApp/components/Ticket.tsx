import * as React from 'react';
import TicketKey from './TicketKey';
import TicketSummary from './TicketSummary';
import TicketAssignee from './TicketAssignee';
import { Issue, Assignee } from './Interfaces';

export default class Ticket extends React.Component<{ issue: Issue }> {
    public render() {

        let currentIssue = this.props.issue;
        let linkToIssue = "https://jira.returnonintelligence.com/browse/" + currentIssue.key;

        return <div>
            <div style={styleKey}><a href={linkToIssue} target="_blank" style={styleLink}> <TicketKey keyName={currentIssue.key} /></a></div>
            <div style={styleAssignee}><TicketAssignee assigneeName={Ticket.AssigneeCheck(currentIssue.fields.assignee)} /></div>
            <div style={styleSummary}><TicketSummary desc={currentIssue.fields.summary} /></div>
        </div>
    }

    private static AssigneeCheck(assignee: Assignee) {

        let AssigneeName;

        if (assignee == null) {
            AssigneeName = "";
        }
        else {
            AssigneeName = assignee.displayName;
        }

        return AssigneeName;
    }
}

const styleLink = {
    color: 'black'
};

const styleAssignee = {
    float: 'right',
    paddingRight: '2%'
};

const styleKey = {
    float: 'left'
};

const styleSummary = {
    clear: 'both'
};