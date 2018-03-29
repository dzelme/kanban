import * as React from 'react';
import TicketStatusName from './TicketStatusName';
import TicketAssignee from './TicketAssignee';
import { Issue, Assignee } from './Interfaces';

export default class TicketInformation extends React.Component<{ issue: Issue }> {
    public render() {

        let currentIssue = this.props.issue;

        return <div className='information'>
            <div><TicketStatusName statusName={currentIssue.fields.status.name} /></div>
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