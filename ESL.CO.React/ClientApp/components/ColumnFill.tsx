import * as React from 'react';
import Ticket from './Ticket';
import { Issue, BoardColumn, Board } from './Interfaces';

interface IColumnFill {
    issueStart: number;
    issueEnd: number;
    issueTotal: number;
}

export default class ColumnFill extends React.Component<{ column: BoardColumn, board: Board, time: number, index: number, columnCount: number }, IColumnFill> {

    timeout: number;

    constructor(props) {
        super(props);
        this.state = {
            issueStart: 0,
            issueEnd: 10,
            issueTotal: this.props.column.issues.length
        };

        this.issueChange = this.issueChange.bind(this);

    }
    
    timeBetweenColumnFill(time: number) {

        let timeTillNext = time/3;

        this.timeout = setTimeout(this.issueChange, timeTillNext);

    }

    issueChange() {

        let issueStart = this.state.issueStart;
        let issueEnd = this.state.issueEnd;
        let issueTotal = this.state.issueTotal


        if (issueEnd == issueTotal) {
            issueStart = 0;
            issueEnd = 10;

        }
        else if ((issueEnd + 10) > issueTotal) {

            let differenceToEnd = issueTotal - issueEnd;

            
            issueStart = issueStart + 10;
            issueEnd = issueEnd + differenceToEnd;


        }
        else {
            issueStart = issueStart + 10;
            issueEnd = issueEnd + 10;
        }

        this.setState({ issueStart: issueStart, issueEnd: issueEnd });

    }

    componentWillUnmount() {
        clearTimeout(this.timeout);
    }
    

    public render() {

        if (this.props.column.issues.length == 0) {
            return null;
        }
        else {

            let SortedIssues = ColumnFill.IssuePriority(this.props.column);

            if (this.props.column.issues.length > 10) {


                if (this.props.index == 0) {

                     let IssuesPerPage = ColumnFill.issueCount(SortedIssues, this.state.issueStart, this.state.issueEnd);

                     let timesToSwap = Math.ceil(this.props.column.issues.length / 10);

                     let timeForOneSwap = this.props.time / timesToSwap;

                     return <div>

                         {
                             IssuesPerPage.map((issue, index) =>

                                 <Ticket issue={issue} />

                             )
                         }

                         {this.timeBetweenColumnFill(timeForOneSwap)}

                     </div>
                }
     
                else {
                    let IssuesPerPage = ColumnFill.issueCount(SortedIssues, 0, 10);

                    return <div>

                        {
                            IssuesPerPage.map((issue, index) =>

                                <Ticket issue={issue} />

                            )
                        }


                    </div>
                }
            }
            else {

                return <div>

                    {
                        SortedIssues.map((issue, index) =>

                            <Ticket issue={issue} />
                        )
                    }

                </div>
            }


        }
    }

        

    private static issueCount(list: Issue[], start: number, end: number) {

        let shortList = [];

        for (var i = start; i < end; i++) {
            shortList.push(list[i]);
        }

        return shortList;
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