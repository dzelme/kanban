import * as React from 'react';
import TicketSummary from './TicketSummary';
import TicketProgress from './TicketProgress';
import TicketInformation from './TicketInformation';
import TicketKey from './TicketKey';
import { Issue } from './Interfaces';


export default class Ticket extends React.Component<{ issue: Issue }> {
    public render() {
        if (this.props.issue.key == '') {
            return null;
        }
        else {
            return <article className={Ticket.PriorityCheck(this.props.issue)}>
                <a href={"https://jira.returnonintelligence.com/browse/" + this.props.issue.key} target="_blank">
                    <div><TicketKey keyName={this.props.issue.key} /></div>
                    <TicketSummary summary={this.props.issue.fields.summary} />
                    <div style={styleProgress}><TicketProgress progress={this.props.issue.fields.progress} created={this.props.issue.fields.created} /></div>
                    <TicketInformation issue={this.props.issue} />
                </a>
            </article>
        }
    }

    private static PriorityCheck(issue: Issue) {
        var Priority = issue.fields.priority.name;
        var PriorityClassName;

        if (Priority == 'Blocker') {
            PriorityClassName = 'box blocker';
        }
        else if (Priority == 'Critical') {
            PriorityClassName = 'box critical';
        }
        else if (Priority == 'Major') {
            PriorityClassName = 'box major';
        }
        else if (Priority == 'Minor') {
            PriorityClassName = 'box minor';
        }
        else if (Priority == 'Trivial') {
            PriorityClassName = 'box trivial';
        }

        return PriorityClassName;
    }
}

const styleProgress = {
    background: 'white',
    color: 'black',
};

const style = {
    color: 'black',
};