﻿import * as React from 'react';
import Ticket from './Ticket';
import { Issue, BoardColumn } from './Interfaces';

export default class ColumnFill extends React.Component<{ column: BoardColumn }> {
    public render() {

        if (this.props.column.issues.length == 0) {
            return null;
        }
        else {

            let SortedIssues = ColumnFill.IssuePriority(this.props.column);

            if (this.props.column.issues.length > 10) {

                let IssuesPerPage = ColumnFill.issueCount(SortedIssues);

                return <div>

                    {
                        IssuesPerPage.map((issue, index) =>

                            <tr key={index}> <td style={ColumnFill.PriorityColor(issue)}><Ticket issue={issue} /></td></tr>

                        )
                    }

                </div>

            }
            else {

                return <div>

                    {

                        SortedIssues.map((issue, index) =>

                            <tr key={index}> <td style={ColumnFill.PriorityColor(issue)}><Ticket issue={issue} /></td></tr>

                        )
                    }

                </div>
            }


        }
    }

    private static issueCount(list: Issue[]) {

        let shortList = [];

        for (var i = 0; i < 10; i++) {
            shortList.push(list[i]);
        }

        return shortList;
    }

    private static PriorityColor(issue: Issue) {
        let Priority = issue.fields.priority.name;
        let Style;

        if (Priority == 'Blocker') {
            Style = styleTicketBlocker;
        }
        else if (Priority == 'Critical') {
            Style = styleTicketCritical;
        }
        else if (Priority == 'Major') {
            Style = styleTicketMajor;
        }
        else if (Priority == 'Minor') {
            Style = styleTicketMinor;
        }
        else if (Priority == 'Trivial') {
            Style = styleTicketTrivial;
        }
        else {
            Style = styleTicket;
        }

        return Style;

    }

    private static IssuePriority(column: BoardColumn) {

        const issueByPriority = [];
        let allIssues = column.issues;

        allIssues.forEach((issue) => {
            if (issue.fields.priority.name == 'Blocker') {
                issueByPriority.push(issue);
            }
        })

        allIssues.forEach((issue) => {
            if (issue.fields.priority.name == 'Critical') {
                issueByPriority.push(issue);
            }
        })

        allIssues.forEach((issue) => {
            if (issue.fields.priority.name == 'Major') {
                issueByPriority.push(issue);
            }
        })

        allIssues.forEach((issue) => {
            if (issue.fields.priority.name == 'Minor') {
                issueByPriority.push(issue);
            }
        })

        allIssues.forEach((issue) => {
            if (issue.fields.priority.name == 'Trivial') {
                issueByPriority.push(issue);
            }
        })

        return issueByPriority;

    }
}


const styleTicket = {
    background: 'yellow',
    borderRadius: '10px',
    paddingLeft: '10px',
    border: 'solid'
};

const styleTicketBlocker = {
    background: 'red',
    borderRadius: '10px',
    paddingLeft: '10px',
    border: 'solid'
};

const styleTicketCritical = {
    background: 'orange',
    borderRadius: '10px',
    paddingLeft: '10px',
    border: 'solid'
};

const styleTicketMajor = {
    background: 'lightyellow',
    borderRadius: '10px',
    paddingLeft: '10px',
    border: 'solid'
};

const styleTicketMinor = {
    background: 'lightgreen',
    borderRadius: '10px',
    paddingLeft: '10px',
    border: 'solid'
};

const styleTicketTrivial = {
    background: 'lightgray',
    borderRadius: '10px',
    paddingLeft: '10px',
    border: 'solid'
};