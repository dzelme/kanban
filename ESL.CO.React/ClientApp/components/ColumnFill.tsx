﻿import * as React from 'react';
import Ticket from './Ticket';
import ColumnTitle from './ColumnTitle';
import { BoardColumn } from './Interfaces';

export default class ColumnFill extends React.Component<{ column: BoardColumn}> {

    public render() {

        return <div className='column-wrapper'>
            <div style={style}><ColumnTitle name={this.props.column.name} issueCount={this.props.column.issues.length} /></div>

            {
                this.props.column.issues.map((issue, index) =>
                    <Ticket issue={issue} key={index} />
                    )
            }
        </div>
    }
}

const style = {
    background: '#555'
}