import * as React from 'react';
import TicketAssignee from './TicketAssignee';
import TicketStatusName from './TicketStatusName';
import TicketKey from './TicketKey';
import { Issue, Assignee } from './Interfaces';

export default class TicketInformation extends React.Component<{ issue: Issue }> {
    public render() {

        let currentIssue = this.props.issue;

        return <div className='information'>
            <div><TicketStatusName statusName={currentIssue.fields.status.name} /></div>
            <div><TicketKey keyName={currentIssue.key} /></div>
            <div><TicketAssignee assigneeName={TicketInformation.AssigneeCheck(currentIssue.fields.assignee)} /></div>
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