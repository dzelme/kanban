﻿import * as React from 'react';

export default class TicketKey extends React.Component<{ keyName: string }> {
    public render() {

        return <div>
            <h4>{this.props.keyName}</h4>
        </div>

    }
}