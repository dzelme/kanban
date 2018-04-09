﻿import * as React from 'react';
import BoardName from './BoardName';
import BoardTable from './BoardTable';
import { ColumnReaderState, Value } from './Interfaces';
import { ApiClient } from './ApiClient';

export default class ColumnReader extends React.Component<{ boardList: Value[], presentationID: string, titleList: string[] }, ColumnReaderState> {
    refreshTimer: number;
    showTimer: number;
    
    constructor(props) {
        super(props);
        this.state = {
            presentationID: "",
            boardList: this.props.boardList,
            currentIndex: 0,
            boardId: this.props.boardList[0].id,
            board: {
                id: "0", name: "", fromCache: false, message: "", columns: [], rows: [], hasChanged: false
            },
            boardChanged: false,
            colorList: [],
            sameBoard: false,
            loading: true
        };

        this.nextSlide = this.nextSlide.bind(this);
        this.boardLoad = this.boardLoad.bind(this);

        ApiClient.getSinglePresentation(this.props.presentationID)
            .then(dataPres => {

                ApiClient.boardData(this.state.boardId, dataPres.credentials)
                    .then(dataBoard => {

                        ApiClient.colorList(this.state.boardId, dataPres.credentials)
                            .then(dataColor => {
                                this.setState({ board: dataBoard, presentationID: this.props.presentationID, loading: false, boardChanged: true, colorList: dataColor, sameBoard: false }, this.RefreshRate);
                            });
                    });
            });     
    }

    nextSlide() {
        clearTimeout(this.refreshTimer);

        var index;

        if (this.state.currentIndex == (this.state.boardList.length - 1)) {
            index = 0;
        }
        else {
            index = this.state.currentIndex + 1;
        }

        const newState = {
            currentIndex: index,
            boardId: this.state.boardList[index].id,
            boardChanged: false
        }

        this.setState(newState, this.boardLoad);
    }

    slideShow() {
        this.updateStatistics();
        this.showTimer = setTimeout(this.nextSlide, this.state.boardList[this.state.currentIndex].timeShown * 1000);
    }

    boardLoad() {

        ApiClient.getSinglePresentation(this.props.presentationID)
            .then(dataPres => {

                ApiClient.boardData(this.state.boardId, dataPres.credentials)
                    .then(dataBoard => {

                        if (dataBoard.id == this.state.boardId) {
                            ApiClient.colorList(this.state.boardId, dataPres.credentials)
                                .then(dataColor => {

                                    if (dataBoard.id == this.state.boardId) {
                                        if (this.state.board.id == dataBoard.id && dataBoard.hasChanged == false) {
                                            this.setState({ boardChanged: false, sameBoard: true }, this.RefreshRate);
                                        }
                                        else if (this.state.board.id == dataBoard.id && dataBoard.hasChanged == true) {
                                            this.setState({ board: dataBoard, colorList: dataColor, boardChanged: true, sameBoard: true }, this.RefreshRate);
                                        }
                                        else {
                                            this.setState({ board: dataBoard, colorList: dataColor, boardChanged: true, sameBoard: false }, this.RefreshRate);
                                        }
                                    }
                                });
                        }
                    });
            });      
    }

    RefreshRate() {
        this.refreshTimer = setTimeout(this.boardLoad, this.state.boardList[this.state.currentIndex].refreshRate * 1000);
    }

    //AD: increments timesShown board statistic
    increment() {
        //ApiClient.saveToStatistics(this.state.board.id.toString(), this.state.board.name);
    }

    shouldComponentUpdate(nextProps, nextState) {
        return nextState.boardChanged;
    }

    componentWillUnmount() {
        clearInterval(this.refreshTimer);
    }

    public render() {

        if (this.state.loading) {
            return <h1>Loading...</h1>
        }
        else {

            if (this.state.board.columns.length == 0) {
                return <h1>Error loading!(Check CAPTCHA)</h1>
            }
            else {
                return <div>

                    <div>  <BoardName presentationId={this.props.presentationID} name={this.state.board.name} fromCache={this.state.board.fromCache} message={this.state.board.message} boardlist={this.state.boardList} /></div>
                    <div id='board'><BoardTable board={this.state.board} colorList={this.state.colorList} /></div>

                    {(this.state.boardList.length == 1) ? this.updateStatistics() : (this.state.sameBoard == false) ? this.slideShow() : this.updateStatistics()} 
                        
                </div>;              
            }
        }
    }
}