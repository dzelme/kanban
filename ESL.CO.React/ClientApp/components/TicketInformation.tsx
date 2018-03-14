import * as React from 'react';
import TicketAssignee from './TicketAssignee';
import TicketStatusName from './TicketStatusName';
import TicketKey from './TicketKey';
import { Issue, Assignee } from './Interfaces';

export default class TicketInformation extends React.Component<{ issue: Issue }> {
    public render() {

        let currentIssue = this.props.issue;

        return <ul /*style={styleInformation}*/ className='information'>
            <li style={styleStatus}><TicketStatusName statusName={currentIssue.fields.status.name} /></li>
            <li style={styleKey}><TicketKey keyName={currentIssue.key} /></li>
            <li style={styleAssignee}><TicketAssignee assigneeName={TicketInformation.AssigneeCheck(currentIssue.fields.assignee)} /></li>
        </ul>

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

const styleInformation = {
    textAlign:'justify'
};

const styleKey = {
    color: 'white',
    display:'inline-block'
};

const styleAssignee = {
    color: 'white',
    display: 'inline-block'
};

const styleStatus = {
    color: 'white',
    display: 'inline-block'
};