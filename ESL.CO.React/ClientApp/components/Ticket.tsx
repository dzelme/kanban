import * as React from 'react';
import TicketSummary from './TicketSummary';
import { Issue, Assignee } from './Interfaces';

export default class Ticket extends React.Component<{ issue: Issue }> {
    public render() {

        let currentIssue = this.props.issue;
        let linkToIssue = "https://jira.returnonintelligence.com/browse/" + currentIssue.key;

        return <div>
        <TicketSummary summary={currentIssue.fields.summary} />
            <ul>
                <li> Backlog </li>
                <li><a href={linkToIssue} target="_blank"> {currentIssue.key} </a></li>
                <li>{Ticket.AssigneeCheck(currentIssue.fields.assignee)} </li>
            </ul>
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