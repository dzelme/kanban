﻿import * as React from 'react';
import TicketKey from './TicketKey';
import TicketSummary from './TicketSummary';
import TicketProgress from './TicketProgress';
import TicketInformation from './TicketInformation';
import { Issue } from './Interfaces';


export default class Ticket extends React.Component<{ issue: Issue }> {
    public render() {
        if (this.props.issue.key == '') {
            return null;
        }
        else {
            return <article className={Ticket.PriorityCheck(this.props.issue)}>
                <a href={"https://jira.returnonintelligence.com/browse/" + this.props.issue.key} target="_blank">
                    <TicketKey keyName={this.props.issue.key} />
                    <TicketSummary summary={this.props.issue.fields.summary} />
                    <TicketProgress progress={this.props.issue.fields.progress} created={this.props.issue.fields.created}/>
                    <TicketInformation issue={this.props.issue} />
                </a>
            </article>
        }
    }

    private static PriorityCheck(issue: Issue) {
        var Priority = issue.fields.priority.name;
        var PriorityClassName;

        if (Priority == 'Blocker') {
            PriorityClassName = 'blocker';
        }
        else if (Priority == 'Critical') {
            PriorityClassName = 'critical';
        }
        else if (Priority == 'Major') {
            PriorityClassName = 'major';
        }
        else if (Priority == 'Minor') {
            PriorityClassName = 'minor';
        }
        else if (Priority == 'Trivial') {
            PriorityClassName = 'trivial';
        }

        return PriorityClassName;
    }
}
