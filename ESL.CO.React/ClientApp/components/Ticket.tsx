import * as React from 'react';
import TicketSummary from './TicketSummary';
import { Issue, Assignee } from './Interfaces';

export default class Ticket extends React.Component<{ issue: Issue }> {
    public render() {

        let currentIssue = this.props.issue;
        let linkToIssue = "https://jira.returnonintelligence.com/browse/" + currentIssue.key;

        return <article className={Ticket.PriorityColor(this.props.issue)}>
            <TicketSummary summary={currentIssue.fields.summary} />

            <ul className = "information">
                <li> {currentIssue.fields.status.name} </li>
                <li><a href={linkToIssue} target="_blank"> {currentIssue.key} </a></li>
                <li>{Ticket.AssigneeCheck(currentIssue.fields.assignee)} </li>
            </ul>
        </article>
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

    private static PriorityColor(issue: Issue) {
        let Priority = issue.fields.priority.name;
        let className;

        if (Priority == 'Blocker') {
            className = "box blocker";
        }
        else if (Priority == 'Critical') {
            className = "box critical";
        }
        else if (Priority == 'Major') {
            className = "box major";
        }
        else if (Priority == 'Minor') {
            className = "box minor";
        }
        else if (Priority == 'Trivial') {
            className = "box trivial";
        }
        else {
            //className = styleTicket;
        }

        return className;

    }
}