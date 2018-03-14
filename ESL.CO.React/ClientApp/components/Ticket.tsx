import * as React from 'react';
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
                    <TicketSummary summary={this.props.issue.fields.summary} />
                    <div style={Ticket.PriorityColor(this.props.issue.fields.priority.name)}><TicketProgress progress={this.props.issue.fields.progress} created={this.props.issue.fields.created} /></div>
                    <TicketInformation issue={this.props.issue} />
                </a>
            </article>
        }
    }

    private static PriorityColor(priority: string) {

        let ProgressStyle;

        if (priority == 'Blocker') {
            ProgressStyle = styleProgressBlocker;
        }
        else if (priority == 'Critical') {
            ProgressStyle = styleProgressCritical;
        }
        else if (priority == 'Major') {
            ProgressStyle = styleProgressMajor;
        }
        else if (priority == 'Minor') {
            ProgressStyle = styleProgressMinor;
        }
        else if (priority == 'Trivial') {
            ProgressStyle = styleProgressTrivial;
        }
        else {
            ProgressStyle = styleProgress;
        }

        return ProgressStyle;
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
    background: '#222',
    color: 'black',
    width:'100%'
};
const styleProgressBlocker = {
    background:'#f44336',
    color: 'white',
    width: '100%'
};
const styleProgressCritical = {
    background: '#ff5722',
    color: 'white',
    width: '100%'
};
const styleProgressMajor = {
    background: '#9c27b0',
    color: 'white',
    width: '100%'
};
const styleProgressMinor = {
    background: '#3f51b5',
    color: 'white',
    width: '100%'
};
const styleProgressTrivial = {
    background: '#607d8b',
    color: 'white',
    width: '100%'
};
